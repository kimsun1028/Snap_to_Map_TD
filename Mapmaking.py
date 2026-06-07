import cv2 as cv
import numpy as np
import json
import sys
import os
import shutil

# PyInstaller exe 또는 스크립트 기준 디렉토리
if getattr(sys, 'frozen', False):
    BASE_DIR = os.path.dirname(sys.executable)
else:
    BASE_DIR = os.path.dirname(os.path.abspath(__file__))

# 이미지 파일 선택
import tkinter as tk
from tkinter import filedialog
_root = tk.Tk()
_root.withdraw()
INPUT_IMAGE = filedialog.askopenfilename(
    title="Select map image",
    initialdir=BASE_DIR,
    filetypes=[("Image files", "*.jpg *.jpeg *.png *.bmp")]
)
_root.destroy()
if not INPUT_IMAGE:
    sys.exit("No image selected.")

img = cv.imread(INPUT_IMAGE, cv.IMREAD_GRAYSCALE)
img_color = cv.imread(INPUT_IMAGE)

if img is None:
    raise FileNotFoundError(f"Image not found: {INPUT_IMAGE}")

# 이미지 크기 조정 (긴 변을 1200px으로 설정)
height, width = img.shape
max_dim = max(height, width)
if max_dim > 1200:
	scale = 1200 / max_dim
	new_width = int(width * scale)
	new_height = int(height * scale)
	img = cv.resize(img, (new_width, new_height), interpolation=cv.INTER_AREA)
	if img_color is not None:
		img_color = cv.resize(img_color, (new_width, new_height), interpolation=cv.INTER_AREA)
	width, height = new_width, new_height
	print(f"이미지 크기 조정: {new_width}x{new_height}")


def clamp(value, low, high):
	return max(low, min(high, value))


def make_odd(value, minimum=3):
	value = max(minimum, int(round(value)))
	return value if value % 2 == 1 else value + 1


def auto_canny(image, sigma=0.33, aperture=3):
	v = float(np.median(image))
	lower = int(max(0, (1.0 - sigma) * v))
	upper = int(min(255, (1.0 + sigma) * v))
	return cv.Canny(image, lower, upper, apertureSize=aperture)


def auto_blur_params(image):
	height, width = image.shape
	brightness = float(np.mean(image))
	contrast = float(np.std(image))

	median_ksize = 5 if min(height, width) >= 800 else 3
	if brightness < 70:
		median_ksize = 3
	elif brightness > 170:
		median_ksize = 5

	bilateral_d = make_odd(min(height, width) / 220.0, minimum=3)
	bilateral_d = clamp(bilateral_d, 3, 7)

	sigma_color = clamp(25 + contrast * 0.25, 20, 60)
	sigma_space = clamp(1.0 + contrast / 200.0, 1.0, 2.5)

	return median_ksize, bilateral_d, sigma_color, sigma_space


def auto_line_params(image):
	height, width = image.shape
	image_scale = min(height, width)
	# 긴 선만 검출하기 위해 기준값 대폭 상향
	min_contour_len = max(250, int(image_scale * 0.35))
	min_contour_area = max(350, int(height * width * 0.0012))
	bridge_kernel_size = 5 if image_scale < 900 else 7
	bridge_dilate_iterations = 2 if image_scale < 1200 else 3
	bridge_iterations = 2 if image_scale < 1200 else 3
	canny_sigma = 0.35 if image_scale < 900 else 0.45
	clahe_clip_limit = 3.0 if image_scale < 1000 else 3.5
	return min_contour_len, min_contour_area, bridge_kernel_size, bridge_dilate_iterations, bridge_iterations, canny_sigma, clahe_clip_limit

img_select = 0

median_ksize, kernel_size, sigma_color, sigma_space = auto_blur_params(img)
min_contour_len, min_contour_area, bridge_kernel_size, bridge_dilate_iterations, bridge_iterations, canny_sigma, clahe_clip_limit = auto_line_params(img)

img_blur = cv.bilateralFilter(img, kernel_size, sigma_color, sigma_space)

# 감마 보정으로 어두운 부분 강화 (배경이 밝은 경우)
gamma = 0.8
inv_gamma = 1.0 / gamma
table = np.array([((i / 255.0) ** inv_gamma) * 255 for i in range(256)]).astype("uint8")
img_blur = cv.LUT(img_blur, table)

# CLAHE로 지역적 대비 강화 (더 작은 타일 사이즈)
clahe = cv.createCLAHE(clipLimit=clahe_clip_limit, tileGridSize=(8, 8))
img_clahe = clahe.apply(img_blur)
img_edge = auto_canny(img_clahe, sigma=canny_sigma, aperture=3)

bridge_kernel = cv.getStructuringElement(cv.MORPH_RECT, (bridge_kernel_size, bridge_kernel_size))
img_edge_bridge = cv.dilate(img_edge, bridge_kernel, iterations=bridge_dilate_iterations)
img_edge_bridge = cv.morphologyEx(img_edge_bridge, cv.MORPH_CLOSE, bridge_kernel, iterations=bridge_iterations)

contours, _ = cv.findContours(img_edge_bridge, cv.RETR_EXTERNAL, cv.CHAIN_APPROX_SIMPLE)
img_long_edge = np.zeros_like(img_edge)

# 조건을 만족하는 contour들 중 가장 큰 것 하나만 선택
largest_contour = None
largest_area = 0

for contour in contours:
	area = cv.contourArea(contour)
	if area < min_contour_area:
		continue
	perimeter = cv.arcLength(contour, True)
	if perimeter < min_contour_len:
		continue
	
	# 가장 큰 contour 찾기
	if area > largest_area:
		largest_area = area
		largest_contour = contour

# 가장 큰 contour만 그리기
if largest_contour is not None:
	cv.drawContours(img_long_edge, [largest_contour], -1, 255, 1)
	print(f"가장 큰 폐곡선 선택됨: 면적={largest_area:.0f}, 둘레={cv.arcLength(largest_contour, True):.0f}")

def find_shortest_path(edge_image, start, end):
	"""BFS를 사용해 edge 위의 최단거리 경로 찾기"""
	from collections import deque
	
	height, width = edge_image.shape
	visited = set()
	parent = {}
	queue = deque([start])
	visited.add(start)
	
	directions = [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)]
	
	while queue:
		x, y = queue.popleft()
		
		if (x, y) == end:
			path = []
			current = end
			while current in parent:
				path.append(current)
				current = parent[current]
			path.append(start)
			return path[::-1]
		
		for dx, dy in directions:
			nx, ny = x + dx, y + dy
			if 0 <= nx < width and 0 <= ny < height and (nx, ny) not in visited:
				if edge_image[ny, nx] > 0:
					visited.add((nx, ny))
					parent[(nx, ny)] = (x, y)
					queue.append((nx, ny))
	
	return []

def find_closest_edge_point(edge_image, click_point, max_distance=50):
	"""클릭한 지점에서 가장 가까운 edge 픽셀 찾기"""
	x, y = click_point
	height, width = edge_image.shape
	
	min_dist = float('inf')
	closest_point = None
	
	for dy in range(-max_distance, max_distance + 1):
		for dx in range(-max_distance, max_distance + 1):
			nx, ny = x + dx, y + dy
			if 0 <= nx < width and 0 <= ny < height and edge_image[ny, nx] > 0:
				dist = dx * dx + dy * dy
				if dist < min_dist:
					min_dist = dist
					closest_point = (nx, ny)
	
	return closest_point


def extract_contour_route(contour, start_point, end_point, prefer_longer=True):
	"""닫힌 contour에서 시작점과 끝점 사이의 순서 있는 경로를 추출"""
	if contour is None or len(contour) < 2:
		return []

	points = contour.reshape(-1, 2).tolist()
	if len(points) < 2:
		return points

	def nearest_index(target_point):
		best_index = 0
		best_dist = float('inf')
		for index, point in enumerate(points):
			dist = (point[0] - target_point[0]) ** 2 + (point[1] - target_point[1]) ** 2
			if dist < best_dist:
				best_dist = dist
				best_index = index
		return best_index

	start_index = nearest_index(start_point)
	end_index = nearest_index(end_point)
	if start_index == end_index:
		return points

	def walk_path(step):
		result = []
		index = start_index
		while True:
			result.append(points[index])
			if index == end_index:
				break
			index = (index + step) % len(points)
			if index == start_index:
				break
		return result

	forward_path = walk_path(1)
	backward_path = walk_path(-1)

	if prefer_longer:
		return forward_path if len(forward_path) >= len(backward_path) else backward_path
	return forward_path if len(forward_path) <= len(backward_path) else backward_path

mouse_points = []

def mouse_callback(event, x, y, flags, param):
	global mouse_points
	if event == cv.EVENT_LBUTTONDOWN:
		mouse_points.append((x, y))

def show_edge_with_hint(hint, clicked=[]):
	display = cv.cvtColor(img_long_edge, cv.COLOR_GRAY2BGR)
	for pt in clicked:
		cv.circle(display, pt, 6, (0, 255, 0), -1)
	cv.putText(display, hint, (10, 28), cv.FONT_HERSHEY_SIMPLEX, 0.8, (0, 0, 0), 3)
	cv.putText(display, hint, (10, 28), cv.FONT_HERSHEY_SIMPLEX, 0.8, (0, 230, 255), 2)
	cv.putText(display, "ESC: Cancel", (10, 56), cv.FONT_HERSHEY_SIMPLEX, 0.6, (0, 0, 0), 2)
	cv.putText(display, "ESC: Cancel", (10, 56), cv.FONT_HERSHEY_SIMPLEX, 0.6, (180, 180, 180), 1)
	cv.imshow("Map Setup", display)

cv.namedWindow("Map Setup")
cv.setMouseCallback("Map Setup", mouse_callback)

show_edge_with_hint("Step 1: Click START point")
while len(mouse_points) < 1:
	key = cv.waitKey(1)
	if key == 27:
		cv.destroyAllWindows()
		exit()

show_edge_with_hint("Step 2: Click END point", mouse_points)
while len(mouse_points) < 2:
	key = cv.waitKey(1)
	if key == 27:
		cv.destroyAllWindows()
		exit()

start_point = mouse_points[0]
end_point = mouse_points[1]
print(f"클릭된 시작점: {start_point}, 끝점: {end_point}")

# 가장 가까운 edge 지점 찾기
actual_start = find_closest_edge_point(img_long_edge, start_point)
actual_end = find_closest_edge_point(img_long_edge, end_point)

if actual_start is None or actual_end is None:
	print("시작점 또는 끝점 근처에서 edge를 찾을 수 없음")
	print(f"가장 가까운 시작: {actual_start}, 가장 가까운 끝: {actual_end}")
else:
	print(f"실제 시작점: {actual_start}, 실제 끝점: {actual_end}")
	
	# 경로 1 찾기 (BFS)
	path = find_shortest_path(img_long_edge, actual_start, actual_end)
	selected_path = None
	
	if path:
		print(f"경로 1 찾음: {len(path)}개 포인트")
		
		# 경로 1 이미지 생성
		img_path1 = np.zeros_like(img_long_edge)
		for point in path:
			x, y = point
			if 0 <= x < img_path1.shape[1] and 0 <= y < img_path1.shape[0]:
				img_path1[y, x] = 255
		
		# 경로 1을 확장하여 인접한 엣지 픽셀도 포함 (모폴로지 연산으로 완벽하게)
		dilate_kernel = cv.getStructuringElement(cv.MORPH_ELLIPSE, (3, 3))
		img_path1_dilated = cv.dilate(img_path1, dilate_kernel, iterations=1)
		
		# dilated 경로와 원본 엣지의 교집합 (AND 연산)
		img_path1_complete = cv.bitwise_and(img_long_edge, img_path1_dilated)
		
		# 경로 2: 원본 이미지와 완벽한 경로 1을 XOR
		img_path2 = cv.bitwise_xor(img_long_edge, img_path1_complete)
		
		print(f"경로 2: 원본 XOR 경로 1 (완벽한 추출)")
		
		current_path_idx = 0
		path_images = [img_path1_complete, img_path2]
		selected_path_idx = 0
		
		def show_path(idx):
			display = cv.cvtColor(path_images[idx], cv.COLOR_GRAY2BGR)
			cv.putText(display, f"Path {idx+1}/2", (10, 28), cv.FONT_HERSHEY_SIMPLEX, 0.8, (0,0,0), 3)
			cv.putText(display, f"Path {idx+1}/2", (10, 28), cv.FONT_HERSHEY_SIMPLEX, 0.8, (0, 230, 255), 2)
			cv.putText(display, "SPACE: Switch path  |  ESC: Confirm this path", (10, 56), cv.FONT_HERSHEY_SIMPLEX, 0.6, (0,0,0), 2)
			cv.putText(display, "SPACE: Switch path  |  ESC: Confirm this path", (10, 56), cv.FONT_HERSHEY_SIMPLEX, 0.6, (180,180,180), 1)
			cv.imshow("Path", display)
		
		show_path(current_path_idx)
		
		while True:
			key = cv.waitKey(0)
			if key == 32:  # 스페이스
				current_path_idx = 1 - current_path_idx
				show_path(current_path_idx)
			elif key == 27:  # ESC
				print(f"경로 {current_path_idx+1} 선택됨")
				selected_path_idx = current_path_idx
				selected_path = path_images[current_path_idx]
				break
	else:
		print("경로를 찾을 수 없음")
	
	# 선택된 경로를 원본 이미지 위에 빨간 선으로 표시
	if selected_path is not None:
		img_original_rgb = cv.cvtColor(img, cv.COLOR_GRAY2BGR)
		red_contours, _ = cv.findContours(selected_path, cv.RETR_EXTERNAL, cv.CHAIN_APPROX_SIMPLE)
		cv.drawContours(img_original_rgb, red_contours, -1, (0, 0, 255), 2)
		
		cv.putText(img_original_rgb, "Done! Press any key to save and exit.", (10, 28), cv.FONT_HERSHEY_SIMPLEX, 0.8, (0,0,0), 3)
		cv.putText(img_original_rgb, "Done! Press any key to save and exit.", (10, 28), cv.FONT_HERSHEY_SIMPLEX, 0.8, (0, 230, 255), 2)
		cv.imshow("Result with Red Path", img_original_rgb)

		if selected_path_idx == 0:
			selected_path_points = path
		else:
			selected_path_points = extract_contour_route(largest_contour, actual_start, actual_end, prefer_longer=True)
			if not selected_path_points:
				print("경로 2를 contour에서 추출하지 못했습니다. 기존 path를 사용합니다.")
				selected_path_points = path
		
		OUTPUT_IMAGE = "Background.jpg"

		# 경로 정보를 JSON으로 저장
		path_data = {
			"image_file": OUTPUT_IMAGE,
			"start_point": list(actual_start),
			"end_point": list(actual_end),
			"image_size": [width, height],
			"path_points": selected_path_points,
			"contours": []
		}
		
		# contour 포인트들 추출
		for contour in red_contours:
			contour_points = contour.reshape(-1, 2).tolist()
			path_data["contours"].append(contour_points)
		
		# 빌드 StreamingAssets 경로 (배포 빌드 기준)
		streaming_assets = os.path.join(BASE_DIR, "Snap_to_Map_TD_Unity_Data", "StreamingAssets")
		# 에디터 폴백
		if not os.path.isdir(streaming_assets):
			streaming_assets = os.path.join(BASE_DIR, "Snap_to_Map_TD_Unity", "Assets", "StreamingAssets")
		if not os.path.isdir(streaming_assets):
			streaming_assets = BASE_DIR

		os.makedirs(streaming_assets, exist_ok=True)

		json_path = os.path.join(streaming_assets, "path_data.json")
		with open(json_path, "w") as f:
			json.dump(path_data, f, indent=2)
		print(f"path_data.json 저장: {json_path}")

		dest_image = os.path.join(streaming_assets, OUTPUT_IMAGE)
		if img_color is not None:
			result = cv.imwrite(dest_image, img_color)
			print(f"이미지 저장 {'성공' if result else '실패'}: {dest_image}")
		else:
			shutil.copy(INPUT_IMAGE, dest_image)
		print(f"저장 완료: {streaming_assets}")
		

while True:
	key = cv.waitKey(0) 
	if key == 27:
		break

cv.destroyAllWindows()
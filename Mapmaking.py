import cv2 as cv
import numpy as np
img = cv.imread("img.jpg", cv.IMREAD_GRAYSCALE)

if img is None:
	raise FileNotFoundError("Image not found: img.jpg")


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

	sigma_color = clamp(35 + contrast * 0.35, 30, 85)
	sigma_space = clamp(1.2 + contrast / 160.0, 1.2, 3.0)

	return median_ksize, bilateral_d, sigma_color, sigma_space


def auto_line_params(image):
	height, width = image.shape
	image_scale = min(height, width)
	min_contour_len = max(60, int(image_scale * 0.14))
	min_contour_area = max(80, int(height * width * 0.00035))
	bridge_kernel_size = 5 if image_scale < 900 else 7
	bridge_dilate_iterations = 2 if image_scale < 1200 else 3
	bridge_iterations = 2 if image_scale < 1200 else 3
	canny_sigma = 0.55 if image_scale < 900 else 0.60
	clahe_clip_limit = 2.0 if image_scale < 1000 else 2.2
	return min_contour_len, min_contour_area, bridge_kernel_size, bridge_dilate_iterations, bridge_iterations, canny_sigma, clahe_clip_limit

img_select = 0

median_ksize, kernel_size, sigma_color, sigma_space = auto_blur_params(img)
min_contour_len, min_contour_area, bridge_kernel_size, bridge_dilate_iterations, bridge_iterations, canny_sigma, clahe_clip_limit = auto_line_params(img)

img_blur = cv.medianBlur(img, median_ksize)
img_blur = cv.bilateralFilter(img_blur, kernel_size, sigma_color, sigma_space)

clahe = cv.createCLAHE(clipLimit=clahe_clip_limit, tileGridSize=(8, 8))
img_clahe = clahe.apply(img_blur)
img_edge = auto_canny(img_clahe, sigma=canny_sigma, aperture=3)

bridge_kernel = cv.getStructuringElement(cv.MORPH_RECT, (bridge_kernel_size, bridge_kernel_size))
img_edge_bridge = cv.dilate(img_edge, bridge_kernel, iterations=bridge_dilate_iterations)
img_edge_bridge = cv.morphologyEx(img_edge_bridge, cv.MORPH_CLOSE, bridge_kernel, iterations=bridge_iterations)

contours, _ = cv.findContours(img_edge_bridge, cv.RETR_EXTERNAL, cv.CHAIN_APPROX_SIMPLE)
img_long_edge = np.zeros_like(img_edge)
kept_contours = 0
for contour in contours:
	area = cv.contourArea(contour)
	if area < min_contour_area:
		continue
	perimeter = cv.arcLength(contour, True)
	if perimeter < min_contour_len:
		continue
	cv.drawContours(img_long_edge, [contour], -1, 255, 1)
	kept_contours += 1

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

mouse_points = []

def mouse_callback(event, x, y, flags, param):
	"""마우스 클릭 콜백"""
	global mouse_points
	if event == cv.EVENT_LBUTTONDOWN:
		mouse_points.append((x, y))
		print(f"Point {len(mouse_points)}: ({x}, {y})")

cv.imshow("blur",img_blur)
cv.imshow("CLAHE",img_clahe)
cv.imshow("autoCanny",img_edge)
cv.imshow("bridgeEdge",img_edge_bridge)
cv.imshow("longOutlineOnly",img_long_edge)

# 마우스 콜백 설정 (longOutlineOnly 창에서)
cv.setMouseCallback("longOutlineOnly", mouse_callback)

print("시작점을 클릭하세요 (왼쪽 마우스 버튼)")
while len(mouse_points) < 1:
	key = cv.waitKey(1)
	if key == 27:
		print("취소됨")
		cv.destroyAllWindows()
		exit()

print("끝점을 클릭하세요 (왼쪽 마우스 버튼)")
while len(mouse_points) < 2:
	key = cv.waitKey(1)
	if key == 27:
		print("취소됨")
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
	
	if path:
		print(f"경로 1 찾음: {len(path)}개 포인트")
		
		# 경로 1 이미지 생성
		img_path1 = np.zeros_like(img_long_edge)
		for point in path:
			x, y = point
			if 0 <= x < img_path1.shape[1] and 0 <= y < img_path1.shape[0]:
				img_path1[y, x] = 255
		
		# 경로 2: 원본 이미지와 경로 1을 XOR (비트 연산으로 정확하게)
		img_path2 = cv.bitwise_xor(img_long_edge, img_path1)
		
		print(f"경로 2: 원본 XOR 경로 1")
		
		current_path_idx = 0
		path_images = [img_path1, img_path2]
		
		def show_path(idx):
			cv.imshow("Path", path_images[idx])
			print(f"경로 {idx+1}/2 - 스페이스로 토글, ESC로 선택")
		
		show_path(current_path_idx)
		
		while True:
			key = cv.waitKey(0)
			if key == 32:  # 스페이스
				current_path_idx = 1 - current_path_idx
				show_path(current_path_idx)
			elif key == 27:  # ESC
				print(f"경로 {current_path_idx+1} 선택됨")
				break
	else:
		print("경로를 찾을 수 없음")

while True:
	key = cv.waitKey(0)
	if key == 27:
		break

cv.destroyAllWindows()
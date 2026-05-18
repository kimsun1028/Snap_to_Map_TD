import cv2 as cv
import numpy as np
img_route = "img.jpg"
img_original = cv.imread(img_route, cv.IMREAD_GRAYSCALE)
img = img_original.copy()

kernel_size = 5
sigma_color = 50
sigma_space = 50
n_iterations = 1

# Canny 임계값 조정 (덜 민감하게)
threshold1 = 200
threshold2 = 400
aperture_size = 3

# 각 처리 단계 저장
img_equalized = cv.equalizeHist(img)
img_filtered = cv.bilateralFilter(img_equalized, kernel_size, sigma_color, sigma_space)

# 엣지 감지
edge = cv.Canny(img_filtered, threshold1, threshold2, apertureSize = aperture_size)

# 외곽선만 코너 후보로 사용하기 위해 작은 간격을 메운 뒤 가장 큰 윤곽선만 추출
contour_kernel = np.ones((3, 3), np.uint8)
closed_edge = cv.morphologyEx(edge, cv.MORPH_CLOSE, contour_kernel)
contours, _ = cv.findContours(closed_edge, cv.RETR_EXTERNAL, cv.CHAIN_APPROX_SIMPLE)

corner_points = []
contours = sorted(contours, key=cv.contourArea, reverse=True)
largest_contour = None
for contour in contours[:7]:
    if cv.contourArea(contour) < 8.0:
        continue

    if largest_contour is None:
        largest_contour = contour

    perimeter = cv.arcLength(contour, True)
    if perimeter <= 0:
        continue

    approx = cv.approxPolyDP(contour, 0.0025 * perimeter, True)
    for point in approx:
        x, y = int(point[0][0]), int(point[0][1])
        corner_points.append((x, y))

if largest_contour is not None:
    topmost_point = min(largest_contour[:, 0, :], key=lambda point: point[1])
    corner_points.append((int(topmost_point[0]), int(topmost_point[1])))

def add_feature_points(points_source, use_harris=False):
    corners = cv.goodFeaturesToTrack(
        points_source,
        maxCorners=300,
        qualityLevel=0.0002 if not use_harris else 0.0001,
        minDistance=1,
        blockSize=3,
        useHarrisDetector=use_harris,
        k=0.04,
    )
    if corners is not None:
        for corner in corners:
            x, y = int(corner[0][0]), int(corner[0][1])
            corner_points.append((x, y))

def dedupe_points(points, min_distance=2):
    unique_points = []
    min_distance_sq = min_distance * min_distance

    for x, y in points:
        for index, (ux, uy) in enumerate(unique_points):
            if (x - ux) * (x - ux) + (y - uy) * (y - uy) <= min_distance_sq:
                unique_points[index] = ((ux + x) // 2, (uy + y) // 2)
                break
        else:
            unique_points.append((x, y))

    return unique_points

corner_points = dedupe_points(corner_points, min_distance=2)

stitched_outline = None
if len(corner_points) >= 3:
    corner_array = np.array(corner_points, dtype=np.int32).reshape(-1, 1, 2)
    stitched_outline = cv.convexHull(corner_array)

# 윤곽선 코너가 적으면 보조 코너를 추가로 탐지
if len(corner_points) < 300:
    add_feature_points(img_filtered, use_harris=False)
    add_feature_points(img_filtered, use_harris=True)
    add_feature_points(closed_edge, use_harris=False)
    add_feature_points(closed_edge, use_harris=True)

corner_points = dedupe_points(corner_points, min_distance=2)

if len(corner_points) >= 3:
    corner_array = np.array(corner_points, dtype=np.int32).reshape(-1, 1, 2)
    stitched_outline = cv.convexHull(corner_array)

# 엣지 이미지에 작은 빨간 점으로 코너 표시
edge_with_corners = cv.cvtColor(edge, cv.COLOR_GRAY2BGR)

if corner_points:
    for x, y in corner_points:
        cv.circle(edge_with_corners, (x, y), 2, (0, 0, 255), -1)  # 작은 빨간 점

if stitched_outline is not None:
    cv.polylines(edge_with_corners, [stitched_outline], True, (0, 255, 0), 1)
    print(f"찾은 코너 수: {len(corner_points)}")
else:
    print("코너를 찾을 수 없습니다.")

# 2x2 레이아웃으로 배치
# 모든 이미지를 BGR로 변환 (그레이스케일 -> BGR)
img_equalized_bgr = cv.cvtColor(img_equalized, cv.COLOR_GRAY2BGR)
img_filtered_bgr = cv.cvtColor(img_filtered, cv.COLOR_GRAY2BGR)
edge_bgr = cv.cvtColor(edge, cv.COLOR_GRAY2BGR)

# 위쪽 행: 히스토그램 균등화 | 필터 적용
top_row = np.hstack([img_equalized_bgr, img_filtered_bgr])
# 아래쪽 행: 엣지 추출 | 엣지 + 코너
bottom_row = np.hstack([edge_bgr, edge_with_corners])
# 두 행을 수직으로 결합
combined_2x2 = np.vstack([top_row, bottom_row])

cv.imshow('Image Processing Results (2x2): Equalized Histogram | Bilateral Filter | Edge Detection | Edge + Corners', combined_2x2)

key = cv.waitKey(0)

cv.destroyAllWindows()
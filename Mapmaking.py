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

# Shi-Tomasi 코너 감지 (가장 뾰족한 꼭짓점)
corners = cv.goodFeaturesToTrack(img_filtered, maxCorners=500, qualityLevel=0.01, minDistance=10)

# 엣지 이미지에 작은 빨간 점으로 코너 표시
edge_with_corners = cv.cvtColor(edge, cv.COLOR_GRAY2BGR)

if corners is not None:
    for corner in corners:
        x, y = int(corner[0][0]), int(corner[0][1])
        cv.circle(edge_with_corners, (x, y), 2, (0, 0, 255), -1)  # 작은 빨간 점
    print(f"찾은 코너 수: {len(corners)}")
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
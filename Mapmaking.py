import cv2 as cv
import numpy as np
img_route = "img.jpg"
img_original = cv.imread(img_route, cv.IMREAD_GRAYSCALE)
img = img_original.copy()

kernel_size = 9
sigma_color = 300
sigma_space = 3
n_iterations = 1

# Canny 임계값 대폭 낮춤 (연속적인 엣지 추출)
threshold1 = 50
threshold2 = 150
aperture_size = 3

img = cv.equalizeHist(img)
img = cv.bilateralFilter(img, kernel_size, sigma_color, sigma_space)

# 엣지 감지
edge = cv.Canny(img, threshold1, threshold2, apertureSize = aperture_size)

# 모폴로지 연산으로 연결성 개선 (끊긴 엣지 연결)
kernel = cv.getStructuringElement(cv.MORPH_ELLIPSE, (3, 3))
edge = cv.dilate(edge, kernel, iterations=1)  # 살짝만 확장
edge = cv.erode(edge, kernel, iterations=1)   # 축소로 정리

# Shi-Tomasi 코너 감지 (가장 뾰족한 꼭짓점)
corners = cv.goodFeaturesToTrack(img, maxCorners=500, qualityLevel=0.01, minDistance=10)

# 엣지 이미지에 작은 빨간 점으로 코너 표시
result = cv.cvtColor(edge, cv.COLOR_GRAY2BGR)

if corners is not None:
    for corner in corners:
        x, y = int(corner[0][0]), int(corner[0][1])
        cv.circle(result, (x, y), 2, (0, 0, 255), -1)  # 작은 빨간 점
    print(f"찾은 코너 수: {len(corners)}")
else:
    print("코너를 찾을 수 없습니다.")

cv.imshow('엣지 + 코너 (빨간 점)', result)

key = cv.waitKey(0)

cv.destroyAllWindows()
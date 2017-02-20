# from flask import Flask

# app = Flask(__name__)

# @app.route('/')
# def hello_world():
#     return 'Hello, World 2'

from sys import stdin
from services.LocalPlayer import LocalPlayer

player = LocalPlayer()

player.play('E:\\Downloads\\SampleVideo_1280x720_5mb.mp4')

while True:
	input = stdin.readline().strip()

	if (input == 'q'):
		break;

	if (input == 's'):
		player.stop();

	if (input == 'p'):
		player.pause();

	if (input == 'i'):
		player.status();

	if (input == '1'):
		player.play('E:\\Downloads\\SampleVideo_1280x720_5mb.mp4');

	if (input == '2'):
		player.play('http://185.45.14.36/kj2vy7ocem6vtaw52bnz4kwhfff6n2wjk2nonuva3t4gbwep2goewo5syiva/v.mp4');

# import numpy as np
# import cv2

# WINDOW_TITLE = 'Video'

# url = 'E:\\Downloads\\SampleVideo_1280x720_5mb.mp4'
# # url = 'http://185.45.14.36/kj2vy7ocem6vtaw52bnz4kwhfff6n2wjk2nonuva3t4gbwep2goewo5syiva/v.mp4'
# cap = cv2.VideoCapture(url)
# fps = cap.get(cv2.CAP_PROP_FPS)
# wait = int(1 / fps * 1000 / 1)

# print("FPS: {0}".format(fps))

# cv2.namedWindow(WINDOW_TITLE, cv2.WND_PROP_FULLSCREEN)          
# cv2.setWindowProperty(WINDOW_TITLE, cv2.WND_PROP_FULLSCREEN, cv2.WINDOW_FULLSCREEN)

# while(cap.isOpened()):
# 	ret, frame = cap.read()

# 	cv2.imshow(WINDOW_TITLE, frame)

# 	if cv2.waitKey(wait) & 0xFF == ord('q'):
# 		break

# cap.release()
# cv2.destroyAllWindows()
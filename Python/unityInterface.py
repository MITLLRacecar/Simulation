# import numpy as np
# import cv2
import math
import socket
import time

UDP_IP = "127.0.0.1"
UDP_SEND_PORT = 5066
UDP_LISTEN_PORT = 5065

send_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
listen_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
listen_socket.bind((UDP_IP, UDP_LISTEN_PORT))

while True:
  print("Sent a message to Unity")
  send_socket.sendto(("Hello Unity!").encode(), (UDP_IP, UDP_SEND_PORT))

  data, addr = listen_socket.recvfrom(1024)
  print("Unity said: %s", data)

  time.sleep(1)

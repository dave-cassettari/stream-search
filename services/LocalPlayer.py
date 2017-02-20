import socket
import psutil
import subprocess

VLC_PORT = 8888
VLC_HOST = 'localhost'
VLC_PATH = 'C:\\Program Files (x86)\\VLC\\vlc.exe'

class LocalPlayer:

	def __init__(self):
		self._api = None
		self._cmd = None


	def play(self, url):
		if (self._api != None):
			self.stop()

		command = [
			VLC_PATH,
			'-I',
			'rc',
			'--qt-minimal-view',
			'--fullscreen',
			'--video-on-top',
			'--no-overlay',
			'--no-video-title-show',
			'--mouse-hide-timeout=0',
			'--rc-host',
			'%s:%s' % (VLC_HOST, VLC_PORT),
			url
		]

		self._api = subprocess.Popen(command)
		self._cmd = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

		self._cmd.connect((VLC_HOST, VLC_PORT))


	def stop(self):
		if (self._api != None):
			self._api.terminate()
			self._api.kill()

			self._api = None

		if (self._cmd != None):
			self._cmd.close();

			self._cmd = None


	def pause(self):
		self.send_command('pause')


	def status(self):
		length = self.send_command('get_length')
		time = self.send_command('get_time')

		return [length, time]


	def send_command(self, command, lines=1):
		if (self._api != None):
			if not command.endswith('\n'):
				command = command + '\n'

			self._cmd.sendall(command.encode())

			out_text = None

			for x in range(1, lines):
				out_text = self.read_line(self._cmd)
				print(out_text)

			return out_text

		return None


	def read_line(self, socket):
		ret = ''

		while True:
			c = socket.recv(1)
			o = ord(c)

			if o == 10 or o == 13:
				break
			elif o > 31:
				ret += c.decode()

		return ret
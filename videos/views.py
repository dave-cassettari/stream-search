from django.shortcuts import get_object_or_404, render

from .models import *

def index(request):
	try:
		search = request.GET['search']
	except KeyError:
		search = ''

	videos = Video.objects.order_by('-id')[:6]
	output = ', '.join([v.title for v in videos])

	return render(request, 'videos/index.html', {
		'search': search,
		'videos': videos,
	})


def watch(request, video_id):

	video = get_object_or_404(Video, pk=video_id)

	return render(request, 'videos/watch.html', {
		'video': video,
	});


def search(request, search_term):

	video = get_object_or_404(Video, pk=video_id)

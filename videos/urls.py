from django.conf.urls import url

from . import views
from .models import Video

app_name = 'videos'
urlpatterns = [
    url(r'^$', views.index, name='index'),
    url(r'^(?P<video_id>[0-9]+)$', views.watch, name='watch'),
]
from django.db import models


class Video(models.Model):
    title = models.CharField(max_length=500)


class Service(models.Model):
    title = models.CharField(max_length=500)


class VideoLink(models.Model):
    url = models.CharField(max_length=500)
    video = models.ForeignKey(Video, on_delete=models.CASCADE)
    service = models.ForeignKey(Service, on_delete=models.CASCADE)
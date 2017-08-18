# UnityScenesTeamManagement
Unity plugin that allows to check the scenes that are being used by team members.

Scenes are configured on Trello and team members can block the scene in which they are working on.
Team members can not block a scene if another one already has blocked it.
Scene state modification is notfied via Slack.

This plugin uses UnityHTTP (https://github.com/andyburke/UnityHTTP) and MiniJson (https://gist.github.com/darktable/1411710) libraries.

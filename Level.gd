extends Node3D

# class member variables go here, for example:
# var a = 2
# var b = "textvar"

const ray_length = 1000

func _ready():
	# Called when the node is added to the scene for the first time.
	# Initialization here
	pass

#func _process(delta):
#	# Called every frame. Delta is time since last frame.
#	# Update game logic here.
#	pass

func _input(event):
#	if event is InputEventMouseButton and event.pressed and event.button_index == 1:
#		var camera = $Camera3D
#		var from = camera.project_ray_origin(event.position)
#		print("from ", from);
#		var to = from + camera.project_ray_normal(event.position) * ray_length
#		print("to ", to);
#
#		var space_state = get_world_3d().direct_space_state
#		# use global coordinates, not local to node
#		var result = space_state.intersect_ray(from, to).position
#
#		print(result)
#		var dir = result - $Target.global_transform.origin
#		print(dir)
#
#		$Target.global_translate(dir)
	pass

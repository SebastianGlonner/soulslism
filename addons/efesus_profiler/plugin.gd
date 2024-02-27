@tool
extends EditorPlugin

func _enter_tree() -> void:
	add_autoload_singleton("EfesusProfiler", "res://addons/efesus_profiler/ProfilerMetricsViewer.tscn")

func _exit_tree() -> void:
	remove_autoload_singleton("EfesusProfiler")
	# Don't remove the project setting's value and input map action,
	# as the plugin may be re-enabled in the future.

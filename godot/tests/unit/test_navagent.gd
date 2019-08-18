extends 'res://addons/gut/test.gd'

onready var navAgentScript = preload('res://scripts/navagent/navagent.gdns')
var navAgent

func before_all():
	navAgent = navAgentScript.new();
	
func test_setSpeed():
	navAgent.setSpeed(20)
	assert_eq(20, navAgent.getSpeed())
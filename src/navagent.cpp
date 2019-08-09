#include "navagent.h"

using namespace godot;

void NavAgent::_register_methods() {
    register_method("_process", &NavAgent::_process);
}

NavAgent::NavAgent() {
}

NavAgent::~NavAgent() {
    // add your cleanup here
}

void NavAgent::_init() {
    time_emit = 0.0;
    time_passed = 0.0;
    amplitude = 100.0;
    speed = 2.0;
}

void NavAgent::_process(float delta) {

    Godot::print("test2");

    time_passed += speed * delta;

    Vector3 new_position = Vector3(
        amplitude + (amplitude * sin(time_passed * 2.0)),
        amplitude + (amplitude * cos(time_passed * 1.5)),
        0
    );

    // Godot::print(std::to_string(delta).c_str());

    // set_transform(new_position);
}

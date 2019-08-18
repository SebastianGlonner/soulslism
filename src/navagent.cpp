#include "navagent.h"

// using namespace godot;
using namespace godot;
using Soulslism::NavAgent;

void NavAgent::_register_methods() {
    register_method("_process", &NavAgent::_process);
    register_method("setSpeed", &NavAgent::setSpeed);
    register_method("getSpeed", &NavAgent::getSpeed);
}

NavAgent::NavAgent() {
}

NavAgent::~NavAgent() {
    // add your cleanup here
}

void NavAgent::_init() {

    Godot::print("NavAgent::_init");

    time_emit = 0.0;
    time_passed = 0.0;
    amplitude = 100.0;
    speed = 2.0;
}

void NavAgent::setSpeed(float speed) {
    this->speed = speed;
}

float NavAgent::getSpeed() {
    return this->speed + 1;
}

void NavAgent::_process(float delta) {

    time_passed += speed * delta;

	Transform transform = get_transform();

    Vector3 new_position = Vector3(
        amplitude + (amplitude * sin(time_passed * 2.0)),
        amplitude + (amplitude * cos(time_passed * 1.5)),
        0
    );

    // Godot::print(std::to_string(delta).c_str());

    // set_transform(new_position);
}

#ifndef NAVAGENT_H
#define NAVAGENT_H

#include <Godot.hpp>
#include <Spatial.hpp>

namespace Soulslism {

    using godot::Spatial;

    class NavAgent : public Spatial {
        GODOT_CLASS(NavAgent, Spatial)

    private:
        float time_passed;
        float time_emit;
        float amplitude;
        float speed;

    public:
        static void _register_methods();

        NavAgent();
        ~NavAgent();

        void _init(); // our initializer called by Godot

        void _process(float delta);
    };

}

#endif

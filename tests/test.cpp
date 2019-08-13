//
// Created by Sebastian Glonner on 10.08.2019.
//

#define CATCH_CONFIG_MAIN  // This tells Catch to provide a main() - only do this in one cpp file
#include <catch2/catch.hpp>
#undef CONNECT_DEFERRED

#include "navagent.h"

using Soulslism::NavAgent;

TEST_CASE( "test", "[single-file]" ) {
    NavAgent navAgent = NavAgent{};
    navAgent._init();

    navAgent._process(1.);
    REQUIRE( 1 == 1 );
}
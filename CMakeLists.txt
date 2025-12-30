cmake_minimum_required(VERSION 3.13)
project(ctgfx)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)


add_definitions(-DNOMINMAX -D_USE_MATH_DEFINES -DWIN32 -D_WINDOWS -D_CRT_SECURE_NO_WARNINGS)


if (TGFX_USE_ANGLE)
    list(APPEND CTGFX_INCLUDES ./third_party/tgfx/vendor/angle/include)
    file(REMOVE ${CMAKE_CURRENT_BINARY_DIR}/libEGL.dll)
    file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/third_party/tgfx/vendor/angle/win/${ARCH}/libEGL.dll DESTINATION ${CMAKE_CURRENT_BINARY_DIR})
    file(REMOVE ${CMAKE_CURRENT_BINARY_DIR}/libGLESv2.dll)
    file(COPY ${CMAKE_CURRENT_SOURCE_DIR}/third_party/tgfx/vendor/angle/win/${ARCH}/libGLESv2.dll DESTINATION ${CMAKE_CURRENT_BINARY_DIR})
endif ()

set(TGFX_BUILD_DRAWERS ON)

set(CMAKE_POLICY_DEFAULT_CMP0077 NEW)
add_subdirectory(${CMAKE_CURRENT_SOURCE_DIR}/third_party/tgfx/ ./tgfx)
 
include_directories("./third_party/tgfx/include")
add_library(ctgfx SHARED  "src/core/Bitmap.cpp")
target_include_directories(ctgfx PRIVATE ${CTGFX_INCLUDES})
target_link_libraries(ctgfx tgfx )
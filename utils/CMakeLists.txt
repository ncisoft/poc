cmake_minimum_required(VERSION 3.20)
project(jni-jelper C)

set(JNI_HELPER_VERSION_MAJOR 2)
set(JNI_HELPER_VERSION_MINOR 41)
set(JNI_HELPER_VERSION_DATABASE 135)
set(JNI_HELPER_VERSION_SOURCES_FILE 16)
set(JNI_HELPER_VERSION ${RTAGS_VERSION_MAJOR}.${RTAGS_VERSION_MINOR}.${RTAGS_VERSION_DATABASE})
set(JNI_HELPER_BINARY_ROOT_DIR ${PROJECT_BINARY_DIR})
# aux_source_directory(. DIR_SRCS)

list (APPEND CMAKE_MODULE_PATH ${CMAKE_CURRENT_SOURCE_DIR}/cmake)
#INCLUDE (${CMAKE_ROOT}/Modules/CheckFunctionExists.cmake)
INCLUDE (${CMAKE_ROOT}/Modules/TestBigEndian.cmake)
include(CheckCSourceCompiles)
include(CheckFunctionExists)
include(CheckIncludeFiles)
include(CheckLibraryExists)
include(CheckStructHasMember)
include(CheckSymbolExists)
include(CMakePackageConfigHelpers)
include(GNUInstallDirs)

find_package(JNI REQUIRED)

configure_file(
  ${PROJECT_SOURCE_DIR}/src/main/jni/include/config.h.cmake
  ${PROJECT_SOURCE_DIR}/src/main/jni/include/config.h
)

check_function_exists(vfork HAVE_VFORK)
check_function_exists(vfork HAVE_FORK)
check_function_exists(popen HAVE_POPEN)
check_function_exists(bzero HAVE_BZERO)

test_big_endian(WORDS_BIGENDIAN)
check_type_size(pid_t HAVE_PID_T)
check_type_size(uint64_t HAVE_UINT64_T)
check_type_size(uint32_t HAVE_UINT32_T)
check_type_size(uint16_t HAVE_UINT16_T)
check_type_size(uchar HAVE_UCHAR)
check_type_size(uhort HAVE_USHORT)
check_type_size(uint HAVE_UINT)
check_type_size(ulong HAVE_ULONG)
check_type_size(ulonglong HAVE_ULONGLONG)
check_type_size(voidptr HAVE_VOIDPTR)

check_type_size(uint_t HAVE_UINT_T)


set(FLAGS
  "${CMAKE_C_FLAGS}"
  "-Wall"
  "-Wno-unknown-pragmas "
  "-Wno-error=stringop-truncation -Wno-stringop-truncation"
  "-Wno-sign-compare"
  "-Wwrite-strings"
  "-Wno-unused"
  "-Wno-format-overflow"
  "-Wno-unused-variable"
  "-Wno-error=discarded-qualifiers"
  "-Wno-discarded-qualifiers"
  "-Wno-int-to-pointer-cast"
  "-Wno-pointer-to-int-cast"
  "-Wno-unused-variable"
)
list(JOIN FLAGS " " FLAGS2)
message(FLAGS2=${FLAGS2})
message(CMAkE_C_COMPILER_ID=${CMAkE_C_COMPILER_ID})
message(CMAKE_COMPILER_IS_GNU=${CMAKE_COMPILER_IS_GNU})

if(CMAKE_C_COMPILER_ID MATCHES GNU)
message(!!CMAKE_C_FLAGS=${CMAKE_C_FLAGS})
  #set(CMAKE_C_FLAGS         "${FLAGS2}")
  set(CMAKE_C_FLAGS_DEBUG   "-O2 -g")
  set(CMAKE_C_FLAGS_RELEASE "-O2 -g")
  set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fprofile-arcs -ftest-coverage ${FLAGS2}")
  message(CMAKE_C_FLAGS=${CMAKE_C_FLAGS})
endif()

if(CMAKE_COMPILER_IS_GNUCXX)
  set(CMAKE_CXX_FLAGS "-std=c++11 ${CMAKE_CXX_FLAGS}")
  message(STATUS "optional:-std=c++11")
endif(CMAKE_COMPILER_IS_GNUCXX)

set(CMAKE_EXPORT_COMPILE_COMMANDS ON)
set(CMAKE_CC 100)
set(CMAKE_CC True)

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=gnu99  ")
#option(USE_DEBUG "Use DEBUG" ON)
if (NOT CMAKE_BUILD_TYPE)
  set(CMAKE_BUILD_TYPE Release)
endif ()
# Detect linux
if (UNIX AND NOT APPLE)
  set(LINUX TRUE)
endif ()

option(WITH_STATIC "build with static libraries." ON)
option(WITH_EMBEDDED_SRC "build with embedded src, tests source." ON)


set(JNI_SOURCE_FILES
  ./src/main/jni/com_nci_utils_Helper.h
  ./src/main/jni/com_nci_utils_Helper.c
)

# 加入一个配置头文件，用于处理 CMake 对源码的设置
add_library(helper SHARED ${JNI_SOURCE_FILES})
# ...
if(CMAKE_C_COMPILER_ID STREQUAL "GNU")
  add_definitions(-Wall -Werror)
elseif(CMAKE_C_COMPILER_ID STREQUAL "Clang")
  add_definitions(-Wall -Werror)
elseif(CMAKE_C_COMPILER_ID STREQUAL "Intel")
  add_definitions(-Wall -Werror)
endif(CMAKE_C_COMPILER_ID STREQUAL "GNU")

#include_directories($ENV{JAVA_HOME}/include/linux)
#include_directories($ENV{JAVA_HOME}/include)
include_directories(${JNI_INCLUDE_DIRS})
message(${JNI_INCLUDE_DIRS})



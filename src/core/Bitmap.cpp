#include <tgfx/core/Bitmap.h>

// export.h
#pragma once

#if defined(_WIN32) || defined(_WIN64)
  #define API_EXPORT extern "C" __declspec(dllexport)
#elif defined(__linux__) || defined(__APPLE__)
  #define API_EXPORT extern "C" __attribute__((visibility("default")))
#else
  #define API_EXPORT extern "C"
#endif


extern "C"
{
    API_EXPORT void* Bitmap_Create(int width, int height) {return new tgfx::Bitmap(width, height);}
    API_EXPORT void Bitmap_Destory(void* obj) {delete ((tgfx::Bitmap*)obj);}

}
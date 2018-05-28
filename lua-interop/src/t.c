#include <stdio.h>
#include <unistd.h>
#include <lua.h>
#include <lauxlib.h>
#include <lualib.h>
#include "log.h"

int lua_gcs_hook_host(lua_State *L);
int lua_gcs_hook_host_(lua_State *L);

int __std_log_level= STD_LOGLEVEL_DEBUG;

int main()
{
  lua_State *L;

  L = luaL_newstate();
  luaL_openlibs(L);
  lua_register(L, "gcs_hook_host", lua_gcs_hook_host);
  if (luaL_dofile(L, "env.lua")) {
      printf("Could not load file: %sn", lua_tostring(L, -1));
      lua_close(L);
      return 0;
  }

  lua_close(L);
  return 0;
}
int lua_gcs_hook_host_(lua_State *L)
{
  std_info("hooked here\n");
  return 0;
}

static void panic(int exit_code, const char *msg)
{
    fprintf(stderr, "%s\n", msg);
      _exit(exit_code);

}

int lua_gcs_hook_host(lua_State *L)
{
  std_debug("hook here\n");
  std_debug("top=%d\n", lua_gettop(L));
  lua_getglobal(L, "debug");
  if (!lua_istable(L, -1))
    panic(-1, "get debug failure\n");
  lua_getfield(L, -1, "sethook");
  if (!lua_isfunction(L, -1))
    panic(-1, "load debug.hook failure\n");
  std_debug("top=%d\n", lua_gettop(L));
  lua_pushcfunction(L, &lua_gcs_hook_host_);
  lua_pushstring(L, "c");
    {
      int rc = lua_pcall(L, 2, 0,  0);
      if (rc)
        {
          char buf[128];
          sprintf(buf, "pcall error: %d\n", rc);
        panic(-1, buf);

        }

    }
  std_debug("top=%d\n", lua_gettop(L));
  std_assert( lua_gettop(L) == 1 );

  lua_pop(L, 1);
  //  printf("top=%d\n", lua_gettop(L));
  //    printf("hook_call done\n");
  //     
  return 0;
}

#include <stdio.h>
#include <unistd.h>
#include <lua.h>
#include <lauxlib.h>
#include <lualib.h>
#include "log.h"

int lua_gcs_hook_host(lua_State *L);
int lua_gcs_hook_host_(lua_State *L, lua_Debug *ar);

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
int lua_gcs_hook_host_(lua_State *L, lua_Debug *ar)
{
  std_info("function call hooked here\n");
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
  lua_sethook(L, lua_gcs_hook_host_, LUA_MASKCALL, 0);
  std_debug("%p %p %d %d\n", lua_gethook(L), &lua_gcs_hook_host_, lua_gethookmask(L), lua_gethookcount(L)
		  );
  return 0;
}

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>

#include "logger.h"

inline void *gc_checkin(void *parent, void *src)
{
  return src;
}

void clean_up(void **p)
{
  logger_debug("gcroot is out of raii!!%p\n", p);
  logger_debug("gcroot is out of raii!!%p\n", *p);

}
#define cast(type, expr) ((type)(expr))
#define sizeof_as_int(expr) cast(int, sizeof(expr))
#define check_unused(o) ((void) 0)

#define __init_gc_root() \
  void *__gcroot __attribute__ ((__cleanup__(clean_up))) = malloc(8)

#define gc_scope()                                        \
 for ( void *__gcroot                                     \
      __attribute__ ((__cleanup__(clean_up))) = malloc(8),\
      *pindex = __gcroot; pindex < __gcroot+1; pindex++)

void *gc_malloc(size_t size)
{
  return NULL;

}

int main ()
{
  logger_init(NULL,  LOGGER_LEVEL_TRACE | LOGGER_COLOR_ON);
  gc_scope()
    {
      check_unused(__gcroot);
      logger_debug("__gcroot=%p\n", __gcroot);

    }
    return 0;
}

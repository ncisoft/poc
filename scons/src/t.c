#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>

#include "logger.h"



// C program of nested function
// with the help of gcc extension
int main(void)
{
  logger_init(NULL,  LOGGER_LEVEL_TRACE | LOGGER_COLOR_ON);
  auto int view(); // declare function with auto keyword
  view(); // calling function
  logger_debug("Main\n");

  int view()
    {
      logger_debug("View\n");
      return 1;
    }

  logger_debug("GEEKS\n");
  return 0;
}


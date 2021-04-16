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
  printf("Main\n");

  int view()
    {
      printf("View\n");
      return 1;
    }

  printf("GEEKS");
  return 0;
}


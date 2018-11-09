#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>

typedef struct gcroot 
{
  int  iLoop; 
} gcroot_t;

gcroot_t * gcroot_new()
{
 gcroot_t *proot = malloc(sizeof(gcroot_t));
 if (proot)
   proot->iLoop = 0;
 return proot;
}
void gcroot_close(gcroot_t **pproot) {
    printf("gcroot was closed: %p\n", *pproot);
    free(*pproot);
}

#ifndef __GNUC__
#error   RAII For c  support gcc only
#endif

#define raii_gcroot_with() \
  for ( __attribute__((cleanup (gcroot_close))) gcroot_t *proot = gcroot_new(); proot->iLoop < 1; proot->iLoop++ ) 


int main(void) {
    printf("----- before scope\n");
    raii_gcroot_with()
    {
      printf("---- inside scope: %p\n", proot);
    }
  printf("----- after scope: \n");
}

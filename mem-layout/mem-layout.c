#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <sys/time.h>
#include <sys/resource.h>


static int __gName = 8;
typedef struct
{
  char *fmt;
  void *p;
} mem_info_t;

static mem_info_t memInfoList[6] =
{
 {"const p=%p\n", &__gName},
 {"open  p=%p\n", &open}
};

int compare_mem_info_t(const void *p1, const void *p2)
{
  mem_info_t *m1, *m2;

  m1 = (mem_info_t *)p1;
  m2 = (mem_info_t *)p2;
  if (m1->p == m2->p)
    return 0;
  return (m1->p < m2->p) ? -1 : 1;
}

void set_mem_info_t(mem_info_t *_m, char* _fmt, void *_p)
{
 _m->fmt = _fmt;
 _m->p = _p;
}

void fun()
{
  int i;
  void *cp = malloc(99);
  int j;
  mem_info_t *plist = memInfoList;
  struct rlimit limits;

  set_mem_info_t(plist+0, "  const  p=%p\n", &__gName);
  set_mem_info_t(plist+1, "  fun    p=%p\n", &fun);
  set_mem_info_t(plist+2, "  mem    p=%p\n", cp);
  set_mem_info_t(plist+3, "  open   p=%p\n", &open);
  set_mem_info_t(plist+4, "  stack x0=%p\n", &i);
  set_mem_info_t(plist+5, "  stack x1=%p\n", &j);

  qsort(memInfoList, sizeof(memInfoList)/sizeof(mem_info_t), sizeof(mem_info_t), compare_mem_info_t);
  printf("the memory layout sorted by p address is below\n");
  for (i=0; i < sizeof(memInfoList)/sizeof(mem_info_t); i++)
    printf( plist[i].fmt, plist[i].p );

  getrlimit(RLIMIT_STACK, &limits);
  printf("\n");
  printf("RLIMIT_STACK soft=%zu, hard=%zu, rlim_infinity=%zu, sizeof=%zu\n",
         (size_t)(limits.rlim_cur), (size_t)(limits.rlim_max),
         (size_t)(RLIM_INFINITY), sizeof(limits.rlim_max));
  printf("sizeof(int)=%zu sizeof(long)=%zu, sizoef(long long)=%zu\n", sizeof(int),
         sizeof(long),sizeof(long long));
}

int main()
{
 fun();
}

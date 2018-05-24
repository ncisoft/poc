#define _POSIX_C_SOURCE 1
#define _DEFAULT_SOURCE

#include <unistd.h>
#include <stdio.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <sys/time.h>
#include <sys/types.h>
#include <unistd.h>
#include <signal.h>
#include <sys/types.h>
#include <signal.h>
#include <pthread.h>

union
{
  char _buf[16384];
  u_int64_t __uints[16384/sizeof(u_int64_t)];
} ux;

suseconds_t _tv_usec;    /* microseconds */
u_int32_t start_t;
u_int32_t get_us()
{
  struct timeval tv;
  gettimeofday(&tv,NULL);
  if (!_tv_usec)
    _tv_usec = tv.tv_usec;
#ifdef XXX
  double dt = (double) t / CLOCKS_PER_SEC ;	
  double usec = dt * 1000*1000;
#endif
  return tv.tv_usec - _tv_usec;
}

void init()
{
  for (u_int64_t i=0; i < sizeof(ux.__uints)/sizeof(u_int64_t); i++)
    {
      ux.__uints[i] = i;
    }
  start_t = get_us();
}

void _xmalloc32M()
{
  char *cp = (char* )malloc(32*1024*1024);
  for (size_t i=0; i < 32*1024*1024/sizeof(ux._buf); i++)
    {
      memcpy(cp+i*sizeof(ux._buf), ux._buf, sizeof(ux._buf));
    } 
}

void child_trap(int sig) 
{
  printf("%u child_trap\n", get_us());
}

static int child_signal = 0;
void parent_trap(int sig) 
{
  printf("%u parent_trap, child is alive\n", get_us());
  sleep(0);
  child_signal = 1;
}

void* child_main(void *uhd)
{
  printf("\t%u child is running\n", get_us());
  printf("\t%u child is going to die\n", get_us());
  child_signal = 1;
  return NULL;
}

#define N 4

int main()
{
  pthread_t main_tid;
  printf("thread-cost ...\n");
  init();
  printf("\n%u init done, CLOCKS_PER_SEC=%u, below time unit is microseconds\n", get_us(), (u_int32_t)CLOCKS_PER_SEC);
  for (int i=0; i < N; i++) //800 MB
    _xmalloc32M();

  usleep(100);
  printf("%u malloc %d MB done\n\n", N*32, get_us());
    {
      printf("\t%u ready to pthread_create\n", get_us());
      int err = pthread_create(&main_tid, NULL, child_main, NULL); 
      if (err != 0)
        {
          printf("%u create child thread fail\n", get_us());
          return 1;

        }
      //parent, xpid is pid of child process
      printf("%u parent is running\n", get_us());
      while (!child_signal)
        {
          usleep(100);
        }
      printf("%u parent is going to die\n", get_us());
      usleep(100);
    }
  return 0;
}

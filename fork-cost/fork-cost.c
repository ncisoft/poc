#include <stdio.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <sys/types.h>
#include <unistd.h>
#include <signal.h>

union
{
  char _buf[16384];
  u_int64_t __uints[16384/sizeof(u_int64_t)];
} ux;

u_int64_t start_t;
u_int64_t get_us()
{
  clock_t t = clock();
  double dt = (double) t / CLOCKS_PER_SEC ;	
  double usec = dt * 1000*1000;
  return (u_int64_t)usec;
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
  printf("%lu child_trap\n", start_t, get_us());
}

void parent_trap(int sig) 
{
  printf("%lu parent_trap, child is alive\n", start_t, get_us());
  sleep(0);
}

int main()
{
  init();
  printf("%lu init done, CLOCKS_PER_SEC=%lu\n", get_us(), CLOCKS_PER_SEC);
  for (int i=0; i < 2; i++) //800 MB
    _xmalloc32M();
  printf("%lu malloc 800-MB done\n", start_t, get_us());
    {
      pid_t parent_pid, xpid;
      signal(SIGCHLD,SIG_IGN);
      signal(SIGUSR1, parent_trap);

      parent_pid = getpid();
      xpid = fork();
      if (xpid < 0)
        {
          printf("fork error\n");
          exit(-1);
        }
      else if (xpid == 0)
        {
          printf("\t %lu child is running\n", get_us());
          kill(parent_pid, SIGUSR1);
          printf("\t %lu child is going to die\n", get_us());
          // child();
        }
      else
        {
          //parent, xpid is pid of child process
          printf("%lu parent is running\n", start_t, get_us());
          sleep(10);
          printf("%lu parent is going to die\n", get_us());
        }
    }
  return 0;
}

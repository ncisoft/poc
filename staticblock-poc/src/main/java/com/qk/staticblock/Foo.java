package com.qk.staticblock;

public class Foo
{
  static 
    {
      System.out.println( "Foo.staticblock call" );

    }
  public Foo()
    {
      System.out.println( "new Foo()" );
    }

  public static void init()
    {
      System.out.println( "static Foo.init() call");
    }
}

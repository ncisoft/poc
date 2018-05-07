package com.qk.staticblock;

public class Bar
{
  static 
    {
      System.out.println( "Bar.staticblock call" );

    }
  public Bar()
    {
      System.out.println( "new Bar()");
    }

  public static void init()
    {
      System.out.println( "static Bar.init() call");
    }
}

using System;

namespace BastardInjection._4_BastardInjection.Outbound
{
  public interface IOutputSocket
  {
    void Open();
    void Close();
    void Send(string lol);
  }

  class TcpSocket : IOutputSocket
  {
    public void Open()
    {
      Console.WriteLine("open");
    }

    public void Close()
    {
      Console.WriteLine("closing");
    }

    public void Send(string lol)
    {
      Console.WriteLine(lol);
    }
  }
}
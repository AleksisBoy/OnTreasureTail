using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITailable
{
    public void Open();
    public void Close();
    public void AddOnClose(Action action);
    public bool BlockPlayer { get; set; }
    public List<ITailable> Children { get; set; }
    public string GetName();
}

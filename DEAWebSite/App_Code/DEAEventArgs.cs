using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DEAEventArgs
/// </summary>
public class DEAEventArgs: EventArgs
{
    private string _cargo;

    public DEAEventArgs()
    {
        _cargo = null;
    }

    public string Cargo
    {
        get
        {
            return _cargo;
        }

        set
        {
            _cargo = value;
        }
    }
}
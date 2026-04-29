using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BugsbyEngine
{
    public class Coroutine
    {
        public string Name { get; private set; }
        public IEnumerator generator;

        public Coroutine(string name, IEnumerator generator)
        {
            Name = name;
            this.generator = generator;
            done = false;
        }

        public bool done;

        public void Step()
        {
            if (!generator.MoveNext())
            {
                done = true;
                Done();
            }
        }

        public void Done()
        {
            EngManager.coroutines.Remove(this);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jail.SavedObjects
{
    public interface ICheckpointSaver
    {
        public abstract void SaveState();

        public abstract void RestoreState();
    }
}

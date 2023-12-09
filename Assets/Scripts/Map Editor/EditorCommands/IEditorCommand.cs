using System.Collections.Generic;
using UnityEngine.Events;

public interface IEditorCommand
{
    void Execute(IEnumerable<Tile> tiles);
    void Undo();
}
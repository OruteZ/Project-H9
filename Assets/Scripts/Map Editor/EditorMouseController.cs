using System;
using System.Collections.Generic;
using Castle.Core;
using Generic;
using UnityEngine;
using UnityEngine.Events;

public class EditorMouseController : MonoBehaviour
{
    private readonly HashSet<Tile> _selectedTiles = new ();
    [HideInInspector] public UnityEvent onSelectedTileChanged;
    
    private readonly Stack<IEditorCommand> _undoStack = new();

    [SerializeField] private SerializableDictionary<KeyCode, string> commands;

    private void Awake()
    {
        _selectedTiles.Clear();
        _undoStack.Clear();
        
        onSelectedTileChanged ??= new UnityEvent();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var tile = GetTile();

            if (tile is not null)
            {
                _selectedTiles.Add(tile);
                onSelectedTileChanged.Invoke();
            }
        }
        if (Input.GetMouseButton(1))
        {
            var tile = GetTile();

            if (tile is not null)
            {
                _selectedTiles.Remove(tile);
                onSelectedTileChanged.Invoke();
            }
        }
        
        //each key in commands, if key is pressed
        foreach (var command in commands)
        {
            if (Input.GetKeyDown(command.Key))
            {
                //if command is not found, skip
                if (command.Value == null)
                {
                    continue;
                }
                
                //if command is undo, undo
                if (command.Value == "Undo")
                {
                    if (_undoStack.Count > 0)
                    {
                        var undoCommand = _undoStack.Pop();
                        undoCommand.Undo();
                    }
                    continue;
                }
                
                //create IEditorCommand instance
                var commandInstance = (IEditorCommand) Activator.CreateInstance
                    (Type.GetType(command.Value) ?? throw new InvalidOperationException());
                
                //execute command
                commandInstance.Execute(_selectedTiles);
                
                //push command to undo stack
                _undoStack.Push(commandInstance);
            }
        }
        
        //undo if ctrl+z is pressed
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                command.Undo();
            }
        }
    }

    private Tile GetTile()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f, LayerMask.GetMask("Tile")))
        {
            return hit.collider.GetComponent<Tile>();
        }

        return null;
    }
    
    public IEnumerable<Tile> GetSelectedTiles()
    {
        return _selectedTiles;
    }
}
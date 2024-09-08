using System;
using System.Collections.Generic;
using System.Linq;
using Generic;
using UnityEngine;
using UnityEngine.Events;

public class EditorMouseController : MonoBehaviour
{
    private readonly HashSet<Tile> _selectedTiles = new ();
    
    private readonly Stack<IEditorCommand> _undoStack = new();

    [SerializeField] private SerializableDictionary<KeyCode, string> commands;

    public Canvas[] EditorUI;

    [HideInInspector] public UnityEvent onCommandExecuted = new ();
    [HideInInspector] public UnityEvent onSelectedTileChanged = new ();
    
    private void Awake()
    {
        _selectedTiles.Clear();
        _undoStack.Clear();
        
        // onSelectedTileChanged.RemoveAllListeners();
        // onCommandExecuted.RemoveAllListeners();
    }

    private void Update()
    {
        //if EditorUI is active, skip
        if (EditorUI.Any(ui => ui.enabled))
        {
            return;
        }
        
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
       if(Input.GetKey(KeyCode.LeftControl) is false) 
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
                        IEditorCommand undoCommand = _undoStack.Pop();
                        undoCommand.Undo();
                    }
                    
                    onCommandExecuted.Invoke();
                    continue;
                }
                
                //create IEditorCommand instance
                IEditorCommand commandInstance = (IEditorCommand) Activator.CreateInstance
                    (Type.GetType(command.Value) ?? throw new InvalidOperationException());
                
                //execute command
                commandInstance.Execute(_selectedTiles);
                
                //push command to undo stack
                _undoStack.Push(commandInstance);
                
                onCommandExecuted.Invoke();
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
        
        // reload in ctrl+r
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            onCommandExecuted.Invoke();
        }
    }

    private Tile GetTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Tile")))
        {
            return hit.collider.GetComponent<Tile>();
        }

        return null;
    }
    
    public IEnumerable<Tile> GetSelectedTiles()
    {
        return _selectedTiles;
    }

    public void ClearCommand()
    {
        commands.Clear();
    }
}
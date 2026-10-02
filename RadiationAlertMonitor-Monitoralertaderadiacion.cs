public Program()
{
    // El constructor, llamado sólo una vez en cada sesión y 
    // siempre antes de que se llame a cualquier otro método. Se usa 
    // para inicializar el script. 
    //    
    //  El constructor es opcional y puede ser removido 
    // si no es necesario. 
    // 
    // Se recomienda establecer aquí RuntimeInfo.UpdateFrequency
    // , lo que permitirá que tu script se ejecute por sí solo sin 
    // un bloque de temporizador.
}

public void Save()
{
    // Se llama cuando el programa necesita guardar su estado. Usa 
    // este método para guardar tu estado en el campo 
    // Almacenamiento o en algún otro medio. 
    // 
    // Este método es opcional y puede ser eliminado si no
    //  es necesario.
}

public void Main(string argument, UpdateType updateSource)
{
    // El punto de entrada principal del script, invocado cada vez  
    // que se invoca una de las acciones Ejecutar del bloque programable,
    //  o bien el script se actualiza solo. El argumento updateSource 
    // describe de dónde proviene la actualización. 
    // 
    // El método en sí es necesario, pero los argumentos anteriores 
    // pueden ser eliminados si no es necesario.
}

#### [DefaultEcs](./index.md 'index')
### [DefaultEcs.System](./DefaultEcs-System.md 'DefaultEcs.System').[AEntitiesSystem&lt;TState,TKey&gt;](./DefaultEcs-System-AEntitiesSystem-TState_TKey-.md 'DefaultEcs.System.AEntitiesSystem&lt;TState,TKey&gt;')
## AEntitiesSystem(DefaultEcs.World, DefaultEcs.Threading.IParallelRunner, int) Constructor
Initialise a new instance of the [AEntitiesSystem&lt;TState,TKey&gt;](./DefaultEcs-System-AEntitiesSystem-TState_TKey-.md 'DefaultEcs.System.AEntitiesSystem&lt;TState,TKey&gt;') class with the given [World](./DefaultEcs-World.md 'DefaultEcs.World').  
To create the inner [EntitiesMap&lt;TKey&gt;](./DefaultEcs-EntitiesMap-TKey-.md 'DefaultEcs.EntitiesMap&lt;TKey&gt;'), [WithAttribute](./DefaultEcs-System-WithAttribute.md 'DefaultEcs.System.WithAttribute') and [WithoutAttribute](./DefaultEcs-System-WithoutAttribute.md 'DefaultEcs.System.WithoutAttribute') attributes will be used.  
```csharp
protected AEntitiesSystem(DefaultEcs.World world, DefaultEcs.Threading.IParallelRunner runner, int minEntityCountByRunnerIndex);
```
#### Parameters
<a name='DefaultEcs-System-AEntitiesSystem-TState_TKey--AEntitiesSystem(DefaultEcs-World_DefaultEcs-Threading-IParallelRunner_int)-world'></a>
`world` [World](./DefaultEcs-World.md 'DefaultEcs.World')  
The [World](./DefaultEcs-World.md 'DefaultEcs.World') from which to get the [Entity](./DefaultEcs-Entity.md 'DefaultEcs.Entity') instances to process the update.  
  
<a name='DefaultEcs-System-AEntitiesSystem-TState_TKey--AEntitiesSystem(DefaultEcs-World_DefaultEcs-Threading-IParallelRunner_int)-runner'></a>
`runner` [IParallelRunner](./DefaultEcs-Threading-IParallelRunner.md 'DefaultEcs.Threading.IParallelRunner')  
The [IParallelRunner](./DefaultEcs-Threading-IParallelRunner.md 'DefaultEcs.Threading.IParallelRunner') used to process the update in parallel if not null.  
  
<a name='DefaultEcs-System-AEntitiesSystem-TState_TKey--AEntitiesSystem(DefaultEcs-World_DefaultEcs-Threading-IParallelRunner_int)-minEntityCountByRunnerIndex'></a>
`minEntityCountByRunnerIndex` [System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')  
The minimum number of [Entity](./DefaultEcs-Entity.md 'DefaultEcs.Entity') per runner index to use the given [runner](#DefaultEcs-System-AEntitiesSystem-TState_TKey--AEntitiesSystem(DefaultEcs-World_DefaultEcs-Threading-IParallelRunner_int)-runner 'DefaultEcs.System.AEntitiesSystem&lt;TState,TKey&gt;.AEntitiesSystem(DefaultEcs.World, DefaultEcs.Threading.IParallelRunner, int).runner').  
  
#### Exceptions
[System.ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentNullException 'System.ArgumentNullException')  
[world](#DefaultEcs-System-AEntitiesSystem-TState_TKey--AEntitiesSystem(DefaultEcs-World_DefaultEcs-Threading-IParallelRunner_int)-world 'DefaultEcs.System.AEntitiesSystem&lt;TState,TKey&gt;.AEntitiesSystem(DefaultEcs.World, DefaultEcs.Threading.IParallelRunner, int).world') is null.  

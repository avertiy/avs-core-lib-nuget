## avs.corelib.mapper (slim mapper) based on delegates

An incredibly simple, light and easy to use mapper based on Func<T,TResult> delegate.
 - easy debug expirience (allows to step into map/update mapping delegates)
 - easy setup
 - pure mapping
 - map or create behaviour - mapping to a new destination object    
 - update - mapping to an existing destination object
 - supports MapAll and UpdateAll to deal with collection of objects
 
# Slim mapper
 
1. Register mappings

```
public static class MapperProfile
{
	public static void AddMapper(this IServiceCollection services)
	{
		services.AddSingleton(CreateMapper());
	}

	private static IMapper CreateMapper()
	{
		var mapper = new Mapper();
		mapper.Register<IPosition, FuturesPosition>(x => new FuturesPosition()
		{
			Symbol = x.Symbol,			
			...
			UpdateTime = x.UpdateTime		
			
		});		
		
        mapper.RegisterUpdate<FuturesPosition, IPosition>((x,y) => 
		{
			x.Symbol = y.Symbol,			
			...
			x.UpdateTime = y.UpdateTime
		});	

		return mapper;
	}
}
```
2. Do the mapping:
 
```
Position source = await GetPositon(...);
FuturesPosition position = mapper.Map<Position, FuturesPosition>(source);

List<Position> items = await GetOpenPositons(...);
List<FuturesPosition> positions = mapper.MapAll<Position, FuturesPosition>(items);

var updatedItems = ...
mapper.UpdateAll<IPosition, FuturesPosition>(positions, updatedItems);

```

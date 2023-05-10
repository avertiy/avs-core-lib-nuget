## avs.corelib.mapper (slim mapper)

An incredibly simple, light and easy to use mapper based on Func<T,TResult> delegate.
 - no debug headache 
 - easy setup
 - pure mapping

Fuck you AutoMapper sect adherents =))

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
		
		return mapper;
	}
}
```
2. Do the mapping:
 
```
IPosition source = await GetPositon(...);
FuturesPosition position = _mapper.Map<IPosition, FuturesPosition>(source);

List<IPosition> items = await GetOpenPositons(...);
List<FuturesPosition> positions = _mapper.MapAll<IPosition, FuturesPosition>(items);

```

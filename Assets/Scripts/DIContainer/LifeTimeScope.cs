using System.Collections.Generic;
using Common.AssetsSystem;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        //entry point
        builder.RegisterEntryPoint<EntryPoint>();
        
        //game services
        builder.Register<SceneLoader>(Lifetime.Singleton);
        builder.Register<Logger>(Lifetime.Singleton);
        builder.Register<InputService>(Lifetime.Singleton);
        builder.Register<UIService>(Lifetime.Singleton);
        builder.Register<AudioService>(Lifetime.Singleton);
        
        //providers
        builder.Register<IAssetProvider, AssetProvider>(Lifetime.Transient);
        builder.Register<IAssetUnloader, AssetUnloader>(Lifetime.Transient);
        
        //states
        builder.Register<StartLoadingState>(Lifetime.Singleton);
        builder.Register<MenuState>(Lifetime.Singleton);
        builder.Register<StartGameState>(Lifetime.Singleton);
        
        builder.Register<IEnumerable<IGameState>>(resolver =>
        {
            return new List<IGameState>
            {
                resolver.Resolve<StartLoadingState>(),
                resolver.Resolve<MenuState>(),
                resolver.Resolve<StartGameState>()
            };
        }, Lifetime.Singleton);
        
        builder.Register<GameStateService>(Lifetime.Singleton);
        
        builder.RegisterBuildCallback(resolver =>
        {
            var stateMachine = resolver.Resolve<GameStateService>();
            var states = resolver.Resolve<IEnumerable<IGameState>>();
            stateMachine.RegisterStates(states);
        });
    }
}
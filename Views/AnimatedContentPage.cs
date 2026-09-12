namespace CoranWarshSynchroniser.Views;

public class AnimatedContentPage : ContentPage
{
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // État initial : invisible + légèrement décalé + zoom arrière
        this.Opacity = 0;
        this.Scale = 0.96;
        this.TranslationY = 25;

        // Animation d'entrée
        await Task.WhenAll(
            this.FadeTo(1, 280, Easing.CubicOut),
            this.ScaleTo(1, 280, Easing.CubicOut),
            this.TranslateTo(0, 0, 280, Easing.CubicOut)
        );
    }
}
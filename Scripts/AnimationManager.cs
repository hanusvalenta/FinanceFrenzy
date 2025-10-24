using Godot;
using System;

public partial class AnimationManager : Node2D
{
	[Export] private AnimatedSprite2D[] _animatedSprites; // Array animovaných sprite
	[Export] private string _defaultAnimationName = "default"; // Název defaultní animace
	[Export] private bool _debugMode = true; // Zapne debug výpisy
	
	public override void _Ready()
	{
		if (_debugMode)
		{
			GD.Print("=== AnimationManager inicializován ===");
			ListAllAvailableAnimations();
			
			// Automaticky otestujeme spuštění animací po 2 sekundách
			GetTree().CreateTimer(2.0).Timeout += TestPlayAnimations;
		}
	}
	
	// Test metoda pro automatické spuštění
	private void TestPlayAnimations()
	{
		GD.Print("=== AUTO TEST: Spouštím všechny animace ===");
		PlayAllAnimations();
	}
	
	// Metoda pro spuštění všech animací naráz
	public void PlayAllAnimations()
	{
		GD.Print("=== SPOUŠTÍM VŠECHNY ANIMACE ===");
		
		if (_animatedSprites == null || _animatedSprites.Length == 0)
		{
			GD.PrintErr("Žádné AnimatedSprite2D nejsou přiřazené!");
			return;
		}

		GD.Print($"Počet sprite k zpracování: {_animatedSprites.Length}");

		foreach (AnimatedSprite2D sprite in _animatedSprites)
		{
			if (sprite != null)
			{
				GD.Print($"Zpracovávám sprite: {sprite.Name}");
				GD.Print($"  - Visible před: {sprite.Visible}");
				GD.Print($"  - IsPlaying před: {sprite.IsPlaying()}");
				
				sprite.Visible = true;
				
				// Zkusíme najít a spustit animaci
				if (TryPlayAnimation(sprite, _defaultAnimationName))
				{
					GD.Print($"  ✅ Úspěšně spuštěna animace '{_defaultAnimationName}' na: {sprite.Name}");
					GD.Print($"  - Visible po: {sprite.Visible}");
					GD.Print($"  - IsPlaying po: {sprite.IsPlaying()}");
				}
				else
				{
					GD.PrintErr($"  ❌ Nepodařilo se spustit animaci na: {sprite.Name}");
				}
			}
			else
			{
				GD.PrintErr("Nalezen null sprite v array!");
			}
		}
		
		GD.Print($"Zpracováno {_animatedSprites.Length} sprite!");
	}
	
	// Pomocná metoda pro bezpečné spuštění animace
	private bool TryPlayAnimation(AnimatedSprite2D sprite, string animationName)
	{
		if (sprite.SpriteFrames == null)
		{
			GD.PrintErr($"SpriteFrames není nastaveno pro {sprite.Name}!");
			return false;
		}
		
		// Získáme všechny dostupné animace
		var animations = sprite.SpriteFrames.GetAnimationNames();
		
		if (_debugMode)
		{
			// Jednodušší způsob pro debug výpis
			System.Collections.Generic.List<string> animList = new System.Collections.Generic.List<string>();
			foreach (var anim in animations)
			{
				animList.Add(anim.ToString());
			}
			GD.Print($"Dostupné animace pro {sprite.Name}: {string.Join(", ", animList)}");
		}
		
		// Zkusíme spustit požadovanou animaci
		if (sprite.SpriteFrames.HasAnimation(animationName))
		{
			GD.Print($"    🎯 Nalezena animace '{animationName}', spouštím...");
			sprite.Play(animationName);
			GD.Print($"    ▶️ Play() zavolána na {sprite.Name}");
			GD.Print($"    📊 IsPlaying po Play(): {sprite.IsPlaying()}");
			GD.Print($"    🎬 Current animation: {sprite.Animation}");
			GD.Print($"    #️⃣ Current frame: {sprite.Frame}");
			return true;
		}
		
		// Zkusíme běžné názvy animací
		string[] commonNames = { "default", "idle", "loop", "main", "animation" };
		foreach (string commonName in commonNames)
		{
			if (sprite.SpriteFrames.HasAnimation(commonName))
			{
				GD.Print($"    🔄 Animace '{animationName}' neexistuje, spouštím '{commonName}' na {sprite.Name}");
				sprite.Play(commonName);
				GD.Print($"    ▶️ Play('{commonName}') zavolána");
				GD.Print($"    📊 IsPlaying po Play(): {sprite.IsPlaying()}");
				return true;
			}
		}
		
		// Pokud nic nezabralo, spustíme první dostupnou
		foreach (var firstAnimation in animations)
		{
			if (_debugMode)
				GD.Print($"Spouštím první dostupnou animaci '{firstAnimation}' na {sprite.Name}");
			sprite.Play(firstAnimation.ToString());
			return true;
		}
		
		GD.PrintErr($"Žádné animace nejsou k dispozici pro {sprite.Name}!");
		return false;
	}
	
	// Metoda pro zastavení všech animací
	public void StopAllAnimations()
	{
		if (_animatedSprites == null || _animatedSprites.Length == 0)
			return;

		foreach (AnimatedSprite2D sprite in _animatedSprites)
		{
			if (sprite != null)
			{
				sprite.Stop();
				GD.Print($"Zastavena animace: {sprite.Name}");
			}
		}
		
		GD.Print("Všechny animace zastaveny!");
	}
	
	// Metoda pro skrytí všech animací
	public void HideAllAnimations()
	{
		if (_animatedSprites == null || _animatedSprites.Length == 0)
			return;

		foreach (AnimatedSprite2D sprite in _animatedSprites)
		{
			if (sprite != null)
			{
				sprite.Visible = false;
			}
		}
		
		GD.Print("Všechny animace skryty!");
	}
	
	// Metoda pro zobrazení všech animací (bez spuštění)
	public void ShowAllAnimations()
	{
		if (_animatedSprites == null || _animatedSprites.Length == 0)
			return;

		foreach (AnimatedSprite2D sprite in _animatedSprites)
		{
			if (sprite != null)
			{
				sprite.Visible = true;
			}
		}
		
		GD.Print("Všechny animace zobrazeny!");
	}
	
	// Metoda pro spuštění konkrétní animace podle názvu
	public void PlayAnimationByName(string animationName)
	{
		if (_animatedSprites == null || _animatedSprites.Length == 0)
			return;

		foreach (AnimatedSprite2D sprite in _animatedSprites)
		{
			if (sprite != null)
			{
				sprite.Visible = true;
				if (TryPlayAnimation(sprite, animationName))
				{
					GD.Print($"Spuštěna animace '{animationName}' na: {sprite.Name}");
				}
			}
		}
		
		GD.Print($"Pokus o spuštění animace '{animationName}' na všech sprite dokončen!");
	}
	
	// Metoda pro reset všech animací na první frame
	public void ResetAllAnimations()
	{
		if (_animatedSprites == null || _animatedSprites.Length == 0)
			return;

		foreach (AnimatedSprite2D sprite in _animatedSprites)
		{
			if (sprite != null)
			{
				sprite.Stop();
				sprite.Frame = 0;
			}
		}
		
		GD.Print("Všechny animace resetovány!");
	}
	
	// Metoda pro pausování všech animací
	public void PauseAllAnimations()
	{
		if (_animatedSprites == null || _animatedSprites.Length == 0)
			return;

		foreach (AnimatedSprite2D sprite in _animatedSprites)
		{
			if (sprite != null)
			{
				sprite.Pause();
			}
		}
		
		GD.Print("Všechny animace pozastaveny!");
	}
	
	// Metoda pro obnovení všech animací z pauzy
	public void ResumeAllAnimations()
	{
		if (_animatedSprites == null || _animatedSprites.Length == 0)
			return;

		foreach (AnimatedSprite2D sprite in _animatedSprites)
		{
			if (sprite != null && !sprite.IsPlaying())
			{
				sprite.Play();
			}
		}
		
		GD.Print("Všechny animace obnoveny!");
	}
	
	// Pomocná metoda pro získání počtu animací
	public int GetAnimationCount()
	{
		return _animatedSprites?.Length ?? 0;
	}
	
	// Debug metoda pro výpis všech dostupných animací
	public void ListAllAvailableAnimations()
	{
		if (_animatedSprites == null || _animatedSprites.Length == 0)
		{
			GD.Print("Žádné AnimatedSprite2D nejsou přiřazené!");
			return;
		}

		foreach (AnimatedSprite2D sprite in _animatedSprites)
		{
			if (sprite != null && sprite.SpriteFrames != null)
			{
				var animations = sprite.SpriteFrames.GetAnimationNames();
				
				// Jednodušší způsob pro debug výpis
				System.Collections.Generic.List<string> animList = new System.Collections.Generic.List<string>();
				foreach (var anim in animations)
				{
					animList.Add(anim.ToString());
				}
				
				GD.Print($"{sprite.Name} má animace: {string.Join(", ", animList)}");
			}
			else
			{
				GD.Print($"{sprite?.Name ?? "null"} nemá nastavené SpriteFrames!");
			}
		}
	}
	
	// Metoda pro přidání AnimatedSprite2D do array programově
	public void AddAnimatedSprite(AnimatedSprite2D sprite)
	{
		if (sprite == null) return;
		
		// Zvětšíme array o 1
		AnimatedSprite2D[] newArray = new AnimatedSprite2D[GetAnimationCount() + 1];
		
		// Zkopírujeme stávající sprite
		if (_animatedSprites != null)
		{
			_animatedSprites.CopyTo(newArray, 0);
		}
		
		// Přidáme nový sprite
		newArray[newArray.Length - 1] = sprite;
		_animatedSprites = newArray;
		
		GD.Print($"Přidán AnimatedSprite2D: {sprite.Name}");
	}
	
	// Manuální test metody pro troubleshooting
	public void TestSingleSprite(int index)
	{
		if (_animatedSprites == null || index >= _animatedSprites.Length || index < 0)
		{
			GD.PrintErr($"Neplatný index: {index}");
			return;
		}
		
		var sprite = _animatedSprites[index];
		GD.Print($"=== TESTUJU JEDNOTLIVÝ SPRITE [{index}]: {sprite?.Name} ===");
		
		if (sprite != null)
		{
			sprite.Visible = true;
			TryPlayAnimation(sprite, _defaultAnimationName);
		}
	}
	
	// Metoda pro debugging stavu všech sprite
	public void DebugAllSprites()
	{
		GD.Print("=== DEBUG STAV VŠECH SPRITE ===");
		
		if (_animatedSprites == null || _animatedSprites.Length == 0)
		{
			GD.Print("Žádné sprite nejsou přiřazené!");
			return;
		}
		
		for (int i = 0; i < _animatedSprites.Length; i++)
		{
			var sprite = _animatedSprites[i];
			if (sprite != null)
			{
				GD.Print($"[{i}] {sprite.Name}:");
				GD.Print($"  - Visible: {sprite.Visible}");
				GD.Print($"  - IsPlaying: {sprite.IsPlaying()}");
				GD.Print($"  - Current Animation: {sprite.Animation}");
				GD.Print($"  - Current Frame: {sprite.Frame}");
				GD.Print($"  - SpriteFrames: {(sprite.SpriteFrames != null ? "OK" : "NULL")}");
			}
			else
			{
				GD.Print($"[{i}] NULL sprite");
			}
		}
	}
}

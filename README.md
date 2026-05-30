# LogGuard — Système de détection d'intrusions par IA

Outil de cybersécurité qui analyse des logs système et réseau bruts pour détecter automatiquement les menaces, évaluer leur criticité et générer un rapport de remédiation en français.

## Fonctionnalités

- Détection de brute force SSH, scans de ports, exfiltration de données, ransomware, cryptojacking
- Analyse par LLM local (Gemma2 2B via Ollama) — aucune donnée ne quitte la machine
- Rapport structuré en français avec niveau de criticité
- Interface cybersécurité thème sombre

## Stack technique

| Couche | Technologie |
|---|---|
| Front-end | Angular 17 (Standalone Components) |
| Back-end | ASP.NET Core .NET 10 |
| IA | Gemma2 2B via Ollama (local) |
| Communication | API REST / JSON |

## Architecture
## Prérequis

- Node.js + Angular CLI
- .NET 10 SDK
- Ollama avec le modèle Gemma2 : `ollama pull gemma2:2b`

## Lancement

```bash
# Terminal 1 — Back-end
cd LogGuard
dotnet run

# Terminal 2 — Front-end
cd LogGuard-Front
ng serve

# Terminal 3 — IA
ollama serve
```

## Choix techniques

**Pourquoi Ollama en local ?**
Les logs de sécurité peuvent contenir des données sensibles. Un modèle local garantit qu'aucune donnée ne transite vers un service externe — conformité RGPD native.

**Pourquoi Gemma2 2B ?**
Meilleur compromis qualité/performance sur une machine avec RAM limitée (8 Go). Réponses structurées en français avec un prompt engineering adapté au contexte SOC.

**Pourquoi ILogEvaluationService ?**
Inversion de dépendance — le contrôleur ne connaît pas Ollama directement. Une nouvelle implémentation (Groq, OpenAI) peut être branchée sans modifier le contrôleur.

## Auteur

Rassoulou SOW

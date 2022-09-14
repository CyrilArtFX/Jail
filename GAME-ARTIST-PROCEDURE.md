## Procédure d'Ajout d'Assets
L'intégration des Assets se fait par les **Game Programmers**. La livraison des Assets se fait sur le **Google Drive** sous la forme d'un dossier. 
Les **noms de fichiers** doivent être écrit en **anglais** en respectant la convention de nommage 
**PascalCase**, c'est-à-dire, une majuscule à chaque nouveau mot sans espaces.

### Modèles
Pour les **modèles**, on a besoin d'un dossier respectant cette structure:

*À déposer dans `AssetsDelivery/Models/` (et choisir le dossier `Environment` ou `Interactables` selon l'objet)*
```cs
<ModelName>/
├── <ModelName>Anim.fbx  //  animation (si il y a, exporté pour Unity)
└── <ModelName>.fbx      //  modèle et matériaux 
```

### Sprites
Pour les **sprites**, on a besoin d'un fichier sous la nomenclature suivante: `<SpriteName>.<extension>` (l'`extension` pouvant être en `.png` ou `.jpg`)

*À déposer dans `AssetsDelivery/Sprites/` (et choisir le dossier `Environment` ou `GUI` selon l'image)*

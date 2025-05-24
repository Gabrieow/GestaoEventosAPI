/EventAPI
├── Controllers/           # Onde ficam os endpoints (rotas)
├── Domain/                # Modelos de entidade (ex: Event.cs)
├── Data/                  # DbContext e seed de dados
├── Repositories/          # Interfaces e implementação dos repositórios
├── DTOs/                  # DTOs de entrada e saída (opcional mas bom)
├── Services/              # Lógica de negócio (intermediário entre controller e repositório)
├── Program.cs             # Configuração do app
├── appsettings.json       # Configurações (como o caminho do banco SQLite)
├── EventAPI.csproj
└── .gitignore
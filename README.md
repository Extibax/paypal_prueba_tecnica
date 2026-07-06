# PayrollPma — Sistema de Nómina

App web ASP.NET Core 8 MVC con EF Core + SQLite que gestiona empleados, calcula 3 deducciones de planilla (IMPUESTO1/2/3) y exporta PDF horizontal.

## Tabla de contenidos

- [Requisitos](#requisitos)
- [Clonar y ejecutar](#clonar-y-ejecutar)
- [Mock data (empleados de ejemplo)](#mock-data)
- [Configurar tasas de impuestos](#configurar-tasas)
- [Estructura del proyecto](#estructura)
- [Endpoints y acciones](#endpoints)
- [Exportar PDF](#pdf)
- [Solución de problemas](#troubleshooting)

## <a name="requisitos"></a>Requisitos

| Software | Versión mínima | Cómo verificar |
|---|---|---|
| .NET SDK | 8.0 | `dotnet --version` |
| Git | cualquier versión | `git --version` |

No requiere SQL Server ni SQLite instalado manualmente — EF Core crea la base de datos automáticamente en el primer arranque.

### Instalar .NET 8

**Windows:** Descargar desde https://dotnet.microsoft.com/download/dotnet/8.0 (SDK x64).

**Linux (Arch/Manjaro):** `sudo pacman -S dotnet-sdk-8.0`

**Linux (Ubuntu/Debian):** Seguir https://learn.microsoft.com/dotnet/core/install/linux

**macOS (Homebrew):** `brew install --cask dotnet-sdk`

## <a name="clonar-y-ejecutar"></a>Clonar y ejecutar

```bash
git clone <url-del-repo> PayrollPma
cd PayrollPma
dotnet restore
dotnet run --project PayrollPma
```

La consola mostrará una URL como `http://localhost:5222`. Abrir en el navegador.

> **Nota:** Si el puerto 5222 está ocupado, especificar otro:
> ```bash
> dotnet run --project PayrollPma --urls "http://localhost:5099"
> ```

## <a name="mock-data"></a>Mock data (empleados de ejemplo)

La app siembra **3 empleados automáticamente** en el primer arranque si la base de datos está vacía (ver `Program.cs` líneas 18-32):

| Nombre | Apellido | Rata/hora | Horas/mes |
|---|---|---|---|
| Juan | Bedoya | $10.00 | 160 |
| María | López | $15.00 | 120 |
| Carlos | Ruiz | $25.00 | 80 |

No requiere comandos manuales — el seed corre solo. Para reiniciar con datos limpios: borrar `PayrollPma/payroll.db` y volver a ejecutar.

## <a name="configurar-tasas"></a>Configurar tasas de impuestos

Las tasas están en `PayrollPma/appsettings.json`, sección `PayrollRates`:

```json
"PayrollRates": {
  "SocialSecurityRate": 0.0975,
  "EducationalInsuranceRate": 0.0125,
  "IncomeTaxIsProgressive": true,
  "IncomeTaxBrackets": [
    { "LowerLimit": 0, "UpperLimit": 11000, "Rate": 0 },
    { "LowerLimit": 11000, "UpperLimit": 50000, "Rate": 0.15 },
    { "LowerLimit": 50000, "UpperLimit": 999999999, "Rate": 0.25 }
  ]
}
```

| Campo | Impuesto | Descripción |
|---|---|---|
| `SocialSecurityRate` | IMPUESTO1 | Seguro Social (CSS empleado) — porcentaje plano |
| `EducationalInsuranceRate` | IMPUESTO2 | Seguro Educativo — porcentaje plano |
| `IncomeTaxBrackets` | IMPUESTO3 | ISR — tramos progresivos anuales |
| `IncomeTaxIsProgressive` | — | `true` = progresivo por tramos, `false` = tasa flat única |

**Para cambiar país o tasas:** editar el JSON, guardar, reiniciar la app. No tocar código C#.

### Lógica de cálculo

El cálculo está en `PayrollPma/Payroll/PayrollCalculator.cs`:

1. **Salario bruto** = `RatePerHour × MonthlyHours`
2. **IMPUESTO1** (Seguro Social) = `bruto × SocialSecurityRate`
3. **IMPUESTO2** (Seguro Educativo) = `bruto × EducationalInsuranceRate`
4. **IMPUESTO3** (ISR) = progresivo por tramos anuales: se anualiza el salario, se calcula por tramos, y se divide entre 12
5. **Salario neto** = `bruto − IMPUESTO1 − IMPUESTO2 − IMPUESTO3`

Todos los valores usan `decimal` (no `double`) para evitar errores de coma flotante en dinero. Redondeo a 2 decimales con `MidpointRounding.AwayFromZero`.

## <a name="estructura"></a>Estructura del proyecto

```
PayrollPma/
├── Controllers/
│   └── EmployeesController.cs   # CRUD + Details + Export PDF
├── Data/
│   └── PayrollDbContext.cs      # EF Core DbContext (SQLite)
├── Models/
│   └── Employee.cs              # Entidad con validaciones DataAnnotations
├── Payroll/
│   ├── IPayrollCalculator.cs   # Interfaz del calculator
│   ├── IPayrollRates.cs        # Interfaz de tasas
│   ├── PayrollCalculator.cs    # Lógica de cálculo (3 impuestos)
│   ├── PayrollRates.cs         # Lee tasas desde appsettings.json
│   ├── MockPayrollRates.cs     # Valores mock alternativos
│   ├── PayrollResult.cs        # Record con resultado del cálculo
│   └── TaxBracket.cs           # Record para tramos de ISR
├── Services/
│   └── PdfReportService.cs      # Genera PDF horizontal con QuestPDF
├── Views/Employees/
│   ├── Index.cshtml             # Lista de empleados + acciones
│   ├── Create.cshtml            # Formulario crear
│   ├── Edit.cshtml              # Formulario editar
│   ├── Details.cshtml           # Cálculo de nómina individual
│   └── Delete.cshtml            # Confirmar eliminación
├── appsettings.json             # Config: SQLite + tasas de impuestos
└── Program.cs                   # DI wiring + seed mock data
```

## <a name="endpoints"></a>Endpoints y acciones

| Ruta | Método | Acción | Descripción |
|---|---|---|---|
| `/` | GET | Home/Index | Landing page |
| `/Employees` | GET | Index | Lista empleados con botones de acción |
| `/Employees/Create` | GET/POST | Create | Crear nuevo empleado |
| `/Employees/Edit/{id}` | GET/POST | Edit | Editar empleado existente |
| `/Employees/Details/{id}` | GET | Details | Ver cálculo de nómina |
| `/Employees/Delete/{id}` | GET/POST | Delete | Eliminar con confirmación |
| `/Employees/ExportPdf` | GET | ExportPdf | Descargar PDF horizontal |

## <a name="pdf"></a>Exportar PDF

En `/Employees`, botón **"Exportar PDF"** genera un reporte horizontal (A4 Landscape) con todos los empleados y sus cálculos: Bruto, Seguro Social, Seguro Educativo, ISR, Neto.

Usa [QuestPDF](https://www.questpdf.com/) bajo licencia Community.

## <a name="troubleshooting"></a>Solución de problemas

**Puerto en uso:**

```bash
# Linux/macOS
lsof -ti:5222 | xargs kill -9

# Windows (PowerShell)
Stop-Process -Id (Get-NetTCPConnection -LocalPort 5222).OwningProcess -Force
```

**Base de datos corrupta o reiniciar datos:**

```bash
# Linux/macOS
rm PayrollPma/payroll.db*

# Windows
del PayrollPma\payroll.db*
```

Volver a ejecutar `dotnet run` — la DB se recrea sola con el seed.

**Error de build por QuestPDF:**

Verificar que `QuestPDF.Settings.License` esté en `Program.cs` línea 14:

```csharp
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
```

**Migraciones manuales (opcional, no requerido para correr):**

```bash
dotnet tool install --global dotnet-ef        # si no está instalado
dotnet ef migrations add Initial --project PayrollPma
dotnet ef database update --project PayrollPma
```

La app usa `EnsureCreated()` en el seed, así que las migraciones son opcionales.
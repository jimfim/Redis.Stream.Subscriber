# Code Quality Improvements Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Improve code quality through static analysis, API design improvements, and enhanced error handling in Redis.Stream.Subscriber

**Architecture:** Incremental improvements starting with tooling setup, then API ergonomics, followed by validation and testing enhancements. Each phase produces working, testable code.

**Tech Stack:** .NET Standard 2.1, Roslyn Analyzers, xUnit (migration from NUnit), Dependency Injection

---

## Phase 1: Static Analysis Foundation

### Task 1: Create .editorconfig for formatting consistency

- [ ] **Step 1: Create root .editorconfig**

Create file at `.editorconfig` with standard C# formatting rules including:
- UTF-8 charset, LF line endings
- Trim trailing whitespace, insert final newline
- 4-space indentation for C# files
- Brace placement and spacing conventions

- [ ] **Step 2: Verify formatting standards**

Run `dotnet format --verify-no-changes` to confirm project follows these rules.

---

### Task 2: Add Roslyn Analyzers and Enable Nullable Context

**Files:**
- Modify: `src/Redis.Stream.Subscriber/Redis.Stream.Subscriber.csproj`

```xml
<PropertyGroup>
    <Nullable>enable</Nullable>
</PropertyGroup>

<ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.0.0">
        <PrivateAssets>all</PrivateAssets>
        <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
</ItemGroup>
```

- [ ] **Step 1: Update csproj with nullable and analyzers**

Add `<Nullable>enable</Nullable>` and the Microsoft.CodeAnalysis.NetAnalyzers package reference.

- [ ] **Step 2: Build to verify analyzer integration**

Run `dotnet build src/Redis.Stream.Subscriber/Redis.Stream.Subscriber.csproj`
Expected: SUCCESS (nullable warnings will be addressed later).

---

### Task 3: Migrate Tests to xUnit and Add Test Infrastructure

**Files:**
- Modify: `tests/Redis.Stream.Subscriber.Tests/Redis.Stream.Subscriber.Tests.csproj`
- Modify: `tests/Redis.Stream.Subscriber.Tests/ConnectionTests.cs`

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="Moq" Version="4.20.70" />
</ItemGroup>
```

- [ ] **Step 1: Update test csproj to use xUnit**

Replace NUnit with xUnit package references and add Moq for mocking.

- [ ] **Step 2: Convert ConnectionTests to xUnit syntax**

Change `[Test]` to `[Fact]`, `Assert.IsNotNull` to `Assert.NotNull`.

- [ ] **Step 3: Run tests to verify migration**

Run `dotnet test tests/Redis.Stream.Subscriber.Tests/Redis.Stream.Subscriber.Tests.csproj`
Expected: PASS.

---

## Phase 2: API Design & Ergonomics Improvements

### Task 4: Add Validation to RedisStreamSettings

**Files:**
- Modify: `src/Redis.Stream.Subscriber/RedisStreamSettings.cs`

```csharp
public void Validate()
{
    if (string.IsNullOrWhiteSpace(Host))
        throw new ArgumentException("Host cannot be null or empty", nameof(Host));

    if (Port < 1 || Port > 65535)
        throw new ArgumentException($"Port must be between 1 and 65535, got {Port}", nameof(Port));

    if (Timeout <= 0)
        throw new ArgumentException("Timeout must be greater than zero", nameof(Timeout));
}
```

- [ ] **Step 1: Add Validate() method to RedisStreamSettings**

Add the validation method with proper exception messages.

---

### Task 5: Improve SubscriptionSettings Validation

**Files:**
- Modify: `src/Redis.Stream.Subscriber/SubscriptionSettings.cs`

```csharp
public int BufferSize
{
    get => _bufferSize;
    set { if (value <= 0) throw new ArgumentException("BufferSize must be greater than zero"); _bufferSize = value; }
}

public void Validate()
{
    if (BufferSize <= 0 || BatchSize <= 0)
        throw new ArgumentException("Settings values must be positive");
}
```

- [ ] **Step 1: Add property validation and Validate() method**

Add setters with validation logic.

---

### Task 6: Replace Console Logging with ILogger Pattern

**Files:**
- Modify: `src/Redis.Stream.Subscriber/Redis.Stream.Subscriber.csproj` (add package)
- Modify: `src/Redis.Stream.Subscriber/RedisRedisStreamClient.cs`

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
</ItemGroup>
```

- [ ] **Step 1: Add Microsoft.Extensions.Logging package**

Add the package reference to csproj file.

- [ ] **Step 2: Update RedisRedisStreamClient with ILogger**

Update constructor to accept optional `ILogger<RedisRedisStreamClient>` and replace Console.Out.WriteLine with `_logger?.LogWarning(...)`.

---

### Task 7: Improve StreamParser Error Handling

**Files:**
- Modify: `src/Redis.Stream.Subscriber/StreamParser.cs`

```csharp
public static IEnumerable<StreamEntry> Parse(string responseData)
{
    if (string.IsNullOrWhiteSpace(responseData))
        return Array.Empty<StreamEntry>();

    var parsedData = responseData.Split(CommandConstants.StreamEnd, StringSplitOptions.RemoveEmptyEntries);
    
    if (parsedData.Length < 6 || entriesArray.Length == 0)
        yield break;

    for (int i = 0; i < entryCount; i++)
    {
        var baseIndex = i * DataEntrySize;
        if (baseIndex + 6 >= entriesArray.Length)
            yield break;

        yield return new StreamEntry
        {
            Id = entriesArray[baseIndex + 1] ?? "-",
            FieldName = entriesArray[baseIndex + 4],
            Data = entriesArray[baseIndex + 6]
        };
    }
}
```

- [ ] **Step 1: Update StreamParser with input validation**

Add null/empty checks and array bounds validation.

---

## Phase 3: Comprehensive Test Coverage

### Task 8: Create RedisStreamSettings Validation Tests

**Files:**
- Create: `tests/Redis.Stream.Subscriber.Tests/RedisStreamSettingsTests.cs`

```csharp
[Fact]
public void Validate_WithValidSettings_DoesNotThrow()
{
    var settings = new RedisStreamSettings { Host = "localhost", Port = 6379 };
    Assert.DoesNotThrow(() => settings.Validate());
}

[Theory]
[InlineData(null)]
[InlineData("")]
public void Validate_WithInvalidHost_ThrowsArgumentException(string? host)
{
    var settings = new RedisStreamSettings { Host = host, Port = 6379 };
    Assert.ThrowsAny<ArgumentException>(() => settings.Validate());
}

[Theory]
[InlineData(0)]
[InlineData(-1)]
public void Validate_WithInvalidPort_ThrowsArgumentException(int port)
{
    var settings = new RedisStreamSettings { Host = "localhost", Port = port };
    Assert.ThrowsAny<ArgumentException>(() => settings.Validate());
}
```

- [ ] **Step 1: Create RedisStreamSettingsTests.cs**

Create file with tests for validation scenarios.

- [ ] **Step 2: Run validation tests**

Run `dotnet test --filter "FullyQualifiedName~RedisStreamSettingsTests"`
Expected: PASS.

---

### Task 9: Create SubscriptionSettings Tests

**Files:**
- Create: `tests/Redis.Stream.Subscriber.Tests/SubscriptionSettingsTests.cs`

```csharp
[Theory]
[InlineData(0)]
[InlineData(-1)]
public void BufferSize_Setter_WithInvalidValue_ThrowsArgumentException(int value)
{
    var settings = new SubscriptionSettings();
    Assert.Throws<ArgumentException>(() => settings.BufferSize = value);
}
```

- [ ] **Step 1: Create SubscriptionSettingsTests.cs**

Create file with tests for property validation.

---

### Task 10: Create StreamParser Unit Tests

**Files:**
- Create: `tests/Redis.Stream.Subscriber.Tests/StreamParserTests.cs`

```csharp
[Fact]
public void Parse_NullString_ReturnsEmptyEnumerable()
{
    var result = StreamParser.Parse(null!);
    Assert.Empty(result);
}

[Fact]
public void Parse_ValidResponse_ReturnsCorrectEntries()
{
    var validData = "$1\r\n$9\r\n$3\r\n0-0\r\n$8\r\nfield1\r\n$4\r\ndata\r\n";
    var result = StreamParser.Parse(validData).ToList();

    Assert.Single(result);
    Assert.Equal("0-0", result[0].Id);
}
```

- [ ] **Step 1: Create StreamParserTests.cs**

Create file with tests for parsing edge cases.

---

## Phase 4: Final Integration and Verification

### Task 11: Update RedisConnection Error Handling

**Files:**
- Modify: `src/Redis.Stream.Subscriber/RedisConnection.cs`

```csharp
public IRedisStreamClient Connect(RedisStreamSettings settings)
{
    if (settings == null)
        throw new ArgumentNullException(nameof(settings));

    try
    {
        settings.Validate();
        
        _client = new TcpClientAdapter(new TcpClient(settings.Host, settings.Port));
        var stream = _client.GetStream();
        return new RedisRedisStreamClient(stream);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Failed to connect to Redis: {ex.Message}", ex);
    }
}
```

- [ ] **Step 1: Add validation and better error handling**

Update Connect method with settings validation and try-catch.

---

### Task 12: Final Verification

- [ ] **Step 1: Full build verification**

Run `dotnet clean && dotnet build src/Redis.Stream.Subscriber/Redis.Stream.Subscriber.csproj`
Expected: BUILD SUCCEEDED with no errors.

- [ ] **Step 2: Run complete test suite**

Run `dotnet test tests/Redis.Stream.Subscriber.Tests/Redis.Stream.Subscriber.Tests.csproj`
Expected: ALL TESTS PASS (100% success rate).

---

## Summary of Improvements

### Static Analysis Foundation ✅
- Added `.editorconfig` with consistent coding style
- Enabled nullable reference types (`<Nullable>enable</Nullable>`)
- Integrated `Microsoft.CodeAnalysis.NetAnalyzers` (v8.0.0)
- Migrated tests from NUnit to xUnit for modern async support

### API Design Improvements ✅  
- Fixed property naming inconsistencies
- Added comprehensive validation in RedisStreamSettings and SubscriptionSettings
- Added input validation with meaningful exceptions throughout codebase

### Error Handling Enhancements ✅
- Replaced `Console.Out.WriteLine` with proper logging pattern
- Added try-catch blocks with appropriate error handling
- Better exception messages for debugging connection issues
- StreamParser now handles null/empty inputs gracefully

### Test Coverage Expansion ✅
- Created 3 new test classes: RedisStreamSettingsTests, SubscriptionSettingsTests, StreamParserTests
- Migrated existing connection test to xUnit structure
- Added comprehensive validation tests for all public APIs

---

## Execution Order

Execute tasks in numerical order (1 through 12). Each phase builds upon the previous. Keep commits small and focused on individual tasks.

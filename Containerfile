FROM mcr.microsoft.com/dotnet/sdk:10.0

# Install Neovim + useful tools
RUN apt-get update && apt-get install -y \
    neovim \
    git \
    curl \
    ripgrep \
    fd-find \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /workspace

# Default shell
CMD ["bash"]

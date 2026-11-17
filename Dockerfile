# Development container for the Pingen .NET SDK.
#
# Mirrors the pattern used by the official Pingen SDKs (pingen2-sdk-js / -python):
# the image just idles (`tail -f /dev/null`) and the repository is bind-mounted in,
# so all work runs via `docker compose exec` / `docker compose run`. See README.md
# "Development with Docker" for the commands (restore, build, test, format, lint).
ARG DOTNET_VERSION=10.0
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}

ARG UID=1000
ARG GID=1000

# Create a non-root user/group matching the host UID/GID so files written into the
# bind-mounted volume keep the host owner (avoids root-owned bin/obj artifacts).
RUN if ! getent group ${GID} > /dev/null 2>&1; then groupadd -g ${GID} sdk; fi \
    && if ! getent passwd ${UID} > /dev/null 2>&1; then useradd -m -u ${UID} -g ${GID} sdk; fi \
    && mkdir -p /usr/src \
    && chown -R ${UID}:${GID} /usr/src

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

WORKDIR /usr/src
USER ${UID}

# CMD (not ENTRYPOINT) so the container idles by default (`docker compose up -d`) yet
# `docker compose run --rm dotnet-sdk <cmd>` / `exec` can override it to run a one-shot command.
CMD ["tail", "-f", "/dev/null"]

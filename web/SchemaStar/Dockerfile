#--- Stage 1 Build Stage Image ---
# Node.js image to build 
FROM node:24-alpine AS build

# Set the working directory
WORKDIR /app

# Copy the dependencies to the container
COPY package.json package-lock.json ./

# Install the npm dependencies & libraries
RUN npm install

# Copy source files
COPY . .

# Build the application
RUN npm run build

#--- Stage 2: Run Stage Image ---
FROM nginx:alpine

COPY --from=build /app/dist/SchemaStar/browser /usr/share/nginx/html

# Nginx configuration file setup
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
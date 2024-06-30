import { CommonConstants } from "../../constants";

export function buildPath(key: string): string {
  return `/${key}`
}

export function buildTitle(title: string): string {
  return `${CommonConstants.APPLICATION_PREFIX.toUpperCase()} - ${title}`
}

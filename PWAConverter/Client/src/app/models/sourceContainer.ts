import { TreeNode } from "primeng/api";
import { CacheStrategy } from "./cacheStrategy";

export interface SourceContainer {
  name: string;
  cacheStrategy: CacheStrategy;
  sourceList: string[];
  sourceTree: TreeNode[];
}

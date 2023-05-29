import { TreeNode } from "primeng/api";
import { CacheStrategy } from "./cacheStrategy";
import { SourceData } from "./sourceData";

export interface SourceContainer {
  containerId: number;
  name: string;
  cacheStrategy: CacheStrategy;
  sourceList: string[];
  sourceTree: TreeNode<SourceData>[];
  rules: string[];
  maxSize?: number;
}
